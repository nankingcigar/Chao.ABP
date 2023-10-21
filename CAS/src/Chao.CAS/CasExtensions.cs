using Microsoft.Extensions.DependencyInjection;
using System;

namespace Chao.CAS;

public static class CasExtensions
{
    public static IServiceCollection AddCAS(this IServiceCollection serviceCollection, Action<CASOption> configureOptions)
    {
        serviceCollection.AddHttpApi<ICASApi>();
        serviceCollection.Configure(configureOptions);
        serviceCollection.AddSingleton<CASHandler>();
        return serviceCollection;
    }
}