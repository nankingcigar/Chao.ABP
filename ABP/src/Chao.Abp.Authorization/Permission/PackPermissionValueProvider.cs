/*
 * @Author: Chao Yang
 * @Date: 2020-11-17 13:33:56
 * @LastEditor: Chao Yang
 * @LastEditTime: 2020-12-06 13:08:39
 */

using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;

namespace Chao.Abp.Authorization.Permission;

public class PackPermissionValueProvider : PermissionValueProvider
{
    public static string ProviderName = "Pack";

    public PackPermissionValueProvider(IPermissionStore permissionStore)
        : base(permissionStore)
    {
    }

    public override string Name => ProviderName;
    public ChaoAbpPermissionOption Option => OptionWrapper.Value;
    public IOptions<ChaoAbpPermissionOption> OptionWrapper { get; set; }

    public override async Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context)
    {
        var packs = context.Principal.Claims?.Where(c => c.Type == Option.PackClaimType).Select(c => c.Value).ToArray();
        return packs.Contains(context.Permission.Name) ? await Task.FromResult(PermissionGrantResult.Granted)
           : await Task.FromResult(PermissionGrantResult.Undefined);
    }

    public override async Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context)
    {
        var packs = context.Principal.Claims?.Where(c => c.Type == Option.PackClaimType).Select(c => c.Value).ToArray();
        var permissionNames = context.Permissions.Select(x => x.Name).Distinct().ToList();
        var result = new MultiplePermissionGrantResult(permissionNames.ToArray());
        foreach (var permissionName in permissionNames)
        {
            if (packs.Contains(permissionName) == true)
            {
                result.Result[permissionName] = PermissionGrantResult.Granted;
            }
            else
            {
                result.Result[permissionName] = PermissionGrantResult.Undefined;
            }
        }
        return await Task.FromResult(result);
    }
}