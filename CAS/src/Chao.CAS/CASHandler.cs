using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Chao.CAS;

public class CASHandler
{
    public CASHandler(
        ICASApi casAPI,
        IOptions<CASOption> casOption
        )
    {
        CASApi = casAPI;
        CASOption = casOption.Value;
    }

    public virtual ICASApi CASApi { get; set; }
    public virtual CASOption CASOption { get; set; }

    public virtual async Task<Profile> GetProfile(string ticket)
    {
        var profile = await CASApi.Get(CASOption.APIUri + ticket);
        return profile;
    }
}