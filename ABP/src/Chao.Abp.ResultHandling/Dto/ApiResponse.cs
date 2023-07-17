/*
 * @Author: Chao Yang
 * @Date: 2020-12-12 08:27:21
 * @LastEditor: Chao Yang
 * @LastEditTime: 2020-12-12 08:53:21
 */

using Volo.Abp.Http;

namespace Chao.Abp.ResultHandling.Dto;

public class ApiResponse : ApiResponse<object>
{
    public ApiResponse(object result)
        : base(result)
    {
    }

    public ApiResponse(RemoteServiceErrorInfo error)
         : base(error)
    {
    }
}