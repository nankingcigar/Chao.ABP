using System.Collections.Generic;

namespace Chao.Abp.CAS;

public class ChaoCASOption
{
    public virtual string? ClientId { get; set; }
    public virtual string? LandingUri { get; set; }
    public virtual IEnumerable<string>? Scope { get; set; }
}