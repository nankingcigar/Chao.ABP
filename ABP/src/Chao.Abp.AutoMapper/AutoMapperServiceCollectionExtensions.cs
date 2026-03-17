using Microsoft.Extensions.DependencyInjection;

namespace Chao.Abp.AutoMapper;

public static class AutoMapperServiceCollectionExtensions
{
    public static IServiceCollection AddAutoMapperWithLicense(
        this IServiceCollection services,
        string licenseKey)
    {
        services.AddAutoMapper(cfg =>
        {
            cfg.LicenseKey = licenseKey;
        });
        return services;
    }
}