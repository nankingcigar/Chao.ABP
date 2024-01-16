using System.Net;

namespace Chao.Abp.AspNetCore.ExceptionHandling;

public class ChaoAbpExceptionHttpStatusCodeOption
{
    public virtual HttpStatusCode? BusinessExceptionErrorCode { get; set; }
}