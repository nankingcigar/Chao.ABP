using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Chao.Abp.AspNetCore.Authentication;

public static class ChaoAbpAuthenticationExtension
{
    public static AuthenticationBuilder AddChaoAuthentication(this IServiceCollection serviceCollection, string schema = ChaoIdentityConst.AuthenticationSchema)
    {
        serviceCollection.AddTransient<ChaoAbpAuthenticationHandler>();
        return serviceCollection.AddAuthentication(options =>
        {
            options.AddScheme<ChaoAbpAuthenticationHandler>(schema, schema);
        });
    }
}