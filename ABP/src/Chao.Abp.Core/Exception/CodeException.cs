/*
 * @Author: Chao Yang
 * @Date: 2020-11-24 02:20:26
 * @LastEditor: Chao Yang
 * @LastEditTime: 2020-11-24 03:06:39
 */

using Microsoft.Extensions.Logging;
using System.Runtime.Serialization;

namespace Chao.Abp.Core.Exception;

public class CodeException : System.Exception, ICodeException
{
    public CodeException(
        string code = null,
        string message = null,
        string details = null,
        System.Exception innerException = null,
        LogLevel logLevel = LogLevel.Warning)
        : base(message, innerException)
    {
        Code = code;
        Details = details;
        LogLevel = logLevel;
    }

    public CodeException(SerializationInfo serializationInfo, StreamingContext context)
        : base(serializationInfo, context)
    {
    }

    public string Code { get; set; }

    public string Details { get; set; }

    public LogLevel LogLevel { get; set; }

    public CodeException WithData(string name, object value)
    {
        Data[name] = value;
        return this;
    }
}