using System.Collections.Generic;
using System.Security.Claims;
using Volo.Abp.DependencyInjection;

namespace Chao.Abp.EventBus.Abstractions;

public class DefaultClaimBuilder : ITransientDependency
{
    public static Dictionary<string, string> ClaimKeyMapping = new() {
        { "sub", ClaimTypes.NameIdentifier },
        { ClaimTypes.NameIdentifier,"sub" },
        { "family_name",ClaimTypes.Surname },
        { ClaimTypes.Surname, "family_name"},
        { "given_name",ClaimTypes.GivenName },
        { ClaimTypes.GivenName,"given_name" },
        { "name",ClaimTypes.Name },
        { ClaimTypes.Name, "name" },
        { "email",ClaimTypes.Email },
        {ClaimTypes.Email, "email" },
        { "role", ClaimTypes.Role },
        { ClaimTypes.Role,"role"}
    };

    public virtual IEnumerable<Claim> Build(Dictionary<string, string> claimDictionary)
    {
        var claims = new List<Claim>();
        foreach (var keyValuePair in claimDictionary)
        {
            claims.Add(new Claim(keyValuePair.Key, keyValuePair.Value));
            if (ClaimKeyMapping.ContainsKey(keyValuePair.Key) == true && claimDictionary.ContainsKey(ClaimKeyMapping[keyValuePair.Key]) == false)
            {
                claims.Add(new Claim(ClaimKeyMapping[keyValuePair.Key], keyValuePair.Value));
            }
        }
        return claims;
    }
}