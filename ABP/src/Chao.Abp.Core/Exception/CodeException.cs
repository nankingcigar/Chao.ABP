/*
 * @Author: Chao Yang
 * @Date: 2020-11-24 02:20:26
 * @LastEditor: Chao Yang
 * @LastEditTime: 2020-11-24 03:06:39
 */

using Microsoft.Extensions.Logging;
using System.Runtime.Serialization;
using Volo.Abp;

namespace Chao.Abp.Core.Exception;

public class CodeException : BusinessException
{
    public CodeException(
        string code = null,
        string message = null,
        string details = null,
        System.Exception innerException = null,
        LogLevel logLevel = LogLevel.Warning)
        : base(code, message, details, innerException, logLevel)
    {
    }

    public CodeException(SerializationInfo serializationInfo, StreamingContext context)
        : base(serializationInfo, context)
    {
    }
}