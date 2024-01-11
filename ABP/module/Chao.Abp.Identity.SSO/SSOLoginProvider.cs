using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;

namespace Chao.Abp.Identity.SSO;

public class SSOLoginProvider : IExternalLoginProvider, ITransientDependency
{
    public virtual ChaoIdentitySSOOption ChaoIdentitySSOOption => ChaoIdentitySSOOptions!.Value;
    public virtual IOptions<ChaoIdentitySSOOption>? ChaoIdentitySSOOptions { get; set; }
    public virtual IdentitySecurityLogManager? IdentitySecurityLogManager { get; set; }
    public virtual IdentityUserStore? IdentityUserStore { get; set; }

    public virtual Task<IdentityUser> CreateUserAsync(string userName, string providerName)
    {
        throw new System.NotImplementedException();
    }

    public virtual async Task<bool> IsEnabledAsync()
    {
        return await Task.FromResult(true);
    }

    public virtual async Task<bool> TryAuthenticateAsync(string userName, string plainPassword)
    {
        if (plainPassword == ChaoIdentitySSOOption.ProviderName)
        {
            var identityUser = await IdentityUserStore!.FindByNameAsync(userName);
            if (identityUser != null)
            {
                return true;
            }
        }
        return false;
    }

    public virtual async Task UpdateUserAsync(IdentityUser user, string providerName)
    {
        await Task.FromResult(true);
    }
}