using System.ComponentModel;
using Volo.Abp.Http;

namespace Chao.Abp.ResultHandling.Dto;

public class ApiResponseError
{
    public ApiResponseError(RemoteServiceErrorInfo error)
    {
        Error = error;
    }

    public bool __chao { get; } = true;
    public RemoteServiceErrorInfo Error { get; set; }

    [DefaultValue(false)]
    public bool Success { get; } = false;
}