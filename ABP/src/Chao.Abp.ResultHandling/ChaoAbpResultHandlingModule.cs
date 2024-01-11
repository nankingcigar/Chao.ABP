using Chao.Abp.ExceptionHandling;
using Volo.Abp.Modularity;

namespace Chao.Abp.ResultHandling;

[DependsOn(typeof(ChaoAbpExceptionHandlingModule))]
public class ChaoAbpResultHandlingModule : AbpModule
{
}