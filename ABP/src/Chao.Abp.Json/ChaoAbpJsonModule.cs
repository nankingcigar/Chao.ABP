/*
 * @Author: Chao Yang
 * @Date: 2020-12-12 01:26:17
 * @LastEditor: Chao Yang
 * @LastEditTime: 2020-12-12 11:56:55
 */

using Volo.Abp.Json;
using Volo.Abp.Modularity;

namespace Chao.Abp.Json;

[DependsOn(
    typeof(ChaoAbpJsonSystemTextJsonModule),
    typeof(AbpJsonModule)
    )]
public class ChaoAbpJsonModule : AbpModule
{
}