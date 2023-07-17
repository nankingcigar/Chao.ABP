using System.Net;
using Volo.Abp.AspNetCore.WebClientInfo;

namespace Chao.Abp.AspNetCore.WebClientInfo;

public static class WebClientInfoProviderExtension
{
    public static string GetComputerName(this IWebClientInfoProvider webClientInfoProvider)
    {
        try
        {
            return Dns.GetHostEntry(IPAddress.Parse(webClientInfoProvider.ClientIpAddress)).HostName;
        }
        catch
        {
            return null;
        }
    }
}