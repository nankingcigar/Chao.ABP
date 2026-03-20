using System;
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace Chao.Abp.WebApiClient;

public class ChaoLoggingFilterAttribue : LoggingFilterAttribute
{
    public int MaxSuccessResponseSizeBytes { get; set; } = 10 * 1024;

    protected override async Task WriteLogAsync(ApiResponseContext context, LogMessage logMessage)
    {
        bool isSuccess = context.HttpContext.ResponseMessage?.IsSuccessStatusCode == true;
        if (isSuccess && MaxSuccessResponseSizeBytes > 0 && logMessage.ResponseContent.IsNullOrEmpty() == false && System.Text.Encoding.UTF8.GetByteCount(logMessage.ResponseContent) > MaxSuccessResponseSizeBytes)
        {
            logMessage.ResponseContent = "...(too large)";
        }
        await base.WriteLogAsync(context, logMessage);
    }
}