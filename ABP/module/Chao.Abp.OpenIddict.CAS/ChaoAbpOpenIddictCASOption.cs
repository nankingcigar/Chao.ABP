using System.Collections.Generic;

namespace Chao.Abp.OpenIddict.CAS;

public class ChaoAbpOpenIddictCASOption
{
    public virtual string ClientId { get; set; }
    public virtual IEnumerable<string> ClientScopes { get; set; }
    public virtual IEnumerable<string> ClientResources { get; set; }
    public virtual string LandingUri { get; set; }
}