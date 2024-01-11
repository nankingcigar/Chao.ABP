using Chao.Abp.Ddd.Domain;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Chao.Abp.EntityFrameworkCore;

[DependsOn(
    typeof(ChaoAbpDddDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
    )]
public class ChaoAbpEntityFrameworkCoreModule : AbpModule
{
}