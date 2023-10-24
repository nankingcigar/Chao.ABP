namespace Chao.Abp.Identity.SSO;

public class ChaoIdentitySSOOption
{
    public virtual string ProviderName { get; set; } = nameof(SSOLoginProvider);
}