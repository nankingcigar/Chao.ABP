/*
 * @Author: Chao Yang
 * @Date: 2020-11-16 17:10:05
 * @LastEditor: Chao Yang
 * @LastEditTime: 2020-11-17 09:02:29
 */

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