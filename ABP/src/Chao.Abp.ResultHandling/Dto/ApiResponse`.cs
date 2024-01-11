namespace Chao.Abp.ResultHandling.Dto;

public class ApiResponse<TResult>(TResult result)
{
#pragma warning disable IDE1006 // 命名样式
    public bool __chao { get; } = true;
#pragma warning restore IDE1006 // 命名样式
    public TResult Result { get; set; } = result;
    public bool Success { get; } = true;
}