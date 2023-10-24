namespace Chao.Abp.CAS;

public class ChaoCASOption
{
    public virtual string ClientId { get; set; }
    public virtual string GrantType { get; set; }
    public virtual string LandingUri { get; set; }
    public virtual string Scope { get; set; }
    public virtual string TokenUri { get; set; }
}