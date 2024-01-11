using Volo.Abp.Http;

namespace Chao.Abp.ResultHandling.Dto;

public class ApiResponse : ApiResponse<object?>
{
    public ApiResponse(object? result)
        : base(result)
    {
    }

    public ApiResponse(RemoteServiceErrorInfo error)
         : base(error)
    {
    }
}