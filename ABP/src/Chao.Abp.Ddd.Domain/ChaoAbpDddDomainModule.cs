/*
 * @Author: Chao Yang
 * @Date: 2020-11-16 16:54:15
 * @LastEditor: Chao Yang
 * @LastEditTime: 2020-11-17 09:02:17
 */

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