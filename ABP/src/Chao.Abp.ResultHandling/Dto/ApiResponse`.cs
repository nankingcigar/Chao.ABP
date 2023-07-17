/*
 * @Author: Chao Yang
 * @Date: 2020-12-12 08:26:20
 * @LastEditor: Chao Yang
 * @LastEditTime: 2020-12-12 08:53:23
 */

namespace Chao.Abp.ResultHandling.Dto;

public class ApiResponse<TResult>
{
    public ApiResponse(TResult result)
    {
        Result = result;
    }

    public bool __chao { get; } = true;
    public TResult Result { get; set; }
    public bool Success { get; } = true;
}