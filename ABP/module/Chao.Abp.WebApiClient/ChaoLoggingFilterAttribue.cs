using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Attributes;
using WebApiClientCore.HttpContents;

namespace Chao.Abp.WebApiClient;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
public class ChaoLoggingFilterAttribue : ApiFilterAttribute
{
    private readonly bool isLoggingFilterAttribute;

    public ChaoLoggingFilterAttribue()
    {
        OrderIndex = int.MaxValue;
        isLoggingFilterAttribute = GetType() == typeof(LoggingFilterAttribute);
    }

    public Func<HttpContent, ValueTask<string?>>? CustomReadRequestContentAsync { get; set; } = null;
    public bool LogRequest { get; set; } = true;
    public bool LogResponse { get; set; } = true;
    public int MaxSuccessResponseSizeBytes { get; set; } = 10 * 1024;

    public override sealed async Task OnRequestAsync(ApiRequestContext context)
    {
        if (context.HttpContext.HttpApiOptions.UseLogging == false)
            return;

        if (isLoggingFilterAttribute && !IsLogEnable(context))
            return;

        var logMessage = new LogMessage
        {
            RequestTime = DateTime.Now,
            HasRequest = LogRequest
        };

        if (LogRequest)
        {
            var request = context.HttpContext.RequestMessage;
            logMessage.RequestHeaders = request.GetHeadersString();
            logMessage.RequestContent = await ReadRequestContentAsync(request, CustomReadRequestContentAsync);
        }

        context.Properties.Set(typeof(LogMessage), logMessage);
    }

    public override sealed async Task OnResponseAsync(ApiResponseContext context)
    {
        var logMessage = context.Properties.Get<LogMessage>(typeof(LogMessage));
        if (logMessage == null)
            return;

        if (isLoggingFilterAttribute)
        {
            if (IsLogEnable(context, out var logger))
            {
                await FillResponseAsync(logMessage, context);
                logMessage.WriteTo(logger);
            }
        }
        else
        {
            await FillResponseAsync(logMessage, context);
            await WriteLogAsync(context, logMessage);
        }
    }

    protected virtual void WriteLog(ILogger logger, LogMessage logMessage)
    {
        logMessage.WriteTo(logger);
    }

    protected virtual Task WriteLogAsync(ApiResponseContext context, LogMessage logMessage)
    {
        var logger = context.GetActionLogger();
        if (logger != null)
            WriteLog(logger, logMessage);
        return Task.CompletedTask;
    }

    private static bool IsLogEnable(ApiRequestContext context)
    {
        var logger = context.GetActionLogger();
        return logger != null && logger.IsEnabled(LogLevel.Error);
    }

    private static bool IsLogEnable(ApiResponseContext context, [MaybeNullWhen(false)] out ILogger logger)
    {
        logger = context.GetActionLogger();
        if (logger == null) return false;

        var logLevel = context.Exception == null ? LogLevel.Information : LogLevel.Error;
        return logger.IsEnabled(logLevel);
    }

    private static async ValueTask<string?> ReadRequestContentAsync(HttpApiRequestMessage request, Func<HttpContent, ValueTask<string?>>? customReadRequestContentAsync)
    {
        if (request.Content == null) return null;
        return customReadRequestContentAsync != null ? await customReadRequestContentAsync(request.Content) :
            request.Content is FormDataContent formDataContent ? await formDataContent.ToCustomHttpContext().ReadAsStringAsync() :
                request.Content is FormDataFileContent formDataFileContent ? await formDataFileContent.ToCustomHttpContext().ReadAsStringAsync() :
                await request.Content.ReadAsStringAsync();
    }

    private static async Task<string?> ReadResponseContentAsync(ApiResponseContext context)
    {
        var content = context.HttpContext.ResponseMessage?.Content;
        if (content == null) return null;

        return content.IsBuffered() == true
            ? await content.ReadAsStringAsync()
            : "...";
    }

    private async Task FillResponseAsync(LogMessage logMessage, ApiResponseContext context)
    {
        logMessage.ResponseTime = DateTime.Now;
        logMessage.Exception = context.Exception;

        var response = context.HttpContext.ResponseMessage;
        if (LogResponse && response != null)
        {
            logMessage.HasResponse = true;
            logMessage.ResponseHeaders = response.GetHeadersString();

            var responseContent = await ReadResponseContentAsync(context);

            if (responseContent != null)
            {
                bool isSuccess = context.HttpContext.ResponseMessage?.IsSuccessStatusCode == true;
                if (isSuccess && MaxSuccessResponseSizeBytes > 0 && System.Text.Encoding.UTF8.GetByteCount(responseContent) > MaxSuccessResponseSizeBytes)
                {
                    logMessage.ResponseContent = "...(too large)";
                }
                else
                {
                    logMessage.ResponseContent = responseContent;
                }
            }
        }
    }
}