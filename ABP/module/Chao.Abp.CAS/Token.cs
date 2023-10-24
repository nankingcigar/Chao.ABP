namespace Chao.Abp.CAS;

public class Token
{
    public virtual string access_token { get; set; }
    public virtual int expires_in { get; set; }
    public virtual string refresh_token { get; set; }
    public virtual string token_type { get; set; }
}