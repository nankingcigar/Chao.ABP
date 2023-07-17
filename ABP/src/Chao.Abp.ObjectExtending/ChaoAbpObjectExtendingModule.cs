using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;

namespace Chao.Abp.ObjectExtending;

[DependsOn(
       typeof(AbpObjectExtendingModule)
       )]
public class ChaoAbpObjectExtendingModule : AbpModule
{
}