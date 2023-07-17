/*
 * @Author: Chao Yang
 * @Date: 2020-11-18 08:54:36
 * @LastEditor: Chao Yang
 * @LastEditTime: 2020-11-18 11:12:24
 */

using Chao.Abp.ExceptionHandling;
using Volo.Abp.Modularity;

namespace Chao.Abp.ResultHandling;

[DependsOn(typeof(ChaoAbpExceptionHandlingModule))]
public class ChaoAbpResultHandlingModule : AbpModule
{
}