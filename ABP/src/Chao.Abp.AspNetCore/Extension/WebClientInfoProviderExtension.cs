using Chao.Abp.AspNetCore.Auditing;
using System;
using System.Net;
using Volo.Abp.AspNetCore.WebClientInfo;

namespace Chao.Abp.AspNetCore.WebClientInfo;

public static class WebClientInfoProviderExtension
{
    private delegate IPHostEntry GetHostEntryHandler(string ip);

    public static string? GetComputerName(this IWebClientInfoProvider webClientInfoProvider, ChaoAbpAspNetCoreAuditingOptions option)
    {
        if (option.DisableAuditClientName == true)
        {
            return null;
        }
        try
        {
            GetHostEntryHandler callback = new(Dns.GetHostEntry);
            IAsyncResult result = callback.BeginInvoke(webClientInfoProvider.ClientIpAddress!, null, null);
            if (result.AsyncWaitHandle.WaitOne(option.DnsResolverTimeoutMillisecond, false))
            {
                return callback.EndInvoke(result).HostName;
            }
            else
            {
                return null;
            }
        }
        catch
        {
            return null;
        }
    }
}