/*
 * @Author: Chao Yang
 * @Date: 2020-11-24 02:20:03
 * @LastEditor: Chao Yang
 * @LastEditTime: 2020-11-24 03:06:42
 */

using Volo.Abp.ExceptionHandling;
using Volo.Abp.Logging;

namespace Chao.Abp.Core.Exception;

public interface ICodeException :
    IHasErrorCode,
    IHasErrorDetails,
    IHasLogLevel
{
}