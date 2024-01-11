using Chao.Abp.ResultHandling;
using Chao.Abp.Timing;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Chao.Abp.Ddd.Domain;

[DependsOn(
    typeof(ChaoAbpTimingModule),
    typeof(ChaoAbpResultHandlingModule),
    typeof(AbpDddDomainModule)
    )]
public class ChaoAbpDddDomainModule : AbpModule
{
}