/*
 * @Author: Chao Yang
 * @Date: 2020-11-17 09:14:52
 * @LastEditor: Chao Yang
 * @LastEditTime: 2020-11-17 10:28:53
 */

using Chao.Abp.EntityFrameworkCore.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ChaoAbpEfCoreServiceCollectionExtensions
{
    public static IServiceCollection AddChaoAbpDbContext<TDbContext>(
        this IServiceCollection services,
        Action<IAbpDbContextRegistrationOptionsBuilder> optionsBuilder = null)
        where TDbContext : AbpDbContext<TDbContext>
    {
        services.AddMemoryCache();
        var options = new AbpDbContextRegistrationOptions(typeof(TDbContext), services);
        var replacedDbContextTypes = typeof(TDbContext).GetCustomAttributes<ReplaceDbContextAttribute>(true)
            .SelectMany(x => x.ReplacedDbContextTypes).ToList();
        foreach (var dbContextType in replacedDbContextTypes)
        {
            options.ReplaceDbContext(dbContextType.Type, multiTenancySides: dbContextType.MultiTenancySide);
        }
        optionsBuilder?.Invoke(options);
        services.TryAddTransient(DbContextOptionsFactory.Create<TDbContext>);
        foreach (var entry in options.ReplacedDbContextTypes)
        {
            var originalDbContextType = entry.Key.Type;
            var targetDbContextType = entry.Value ?? typeof(TDbContext);

            services.Replace(ServiceDescriptor.Transient(originalDbContextType, sp =>
            {
                var dbContextType = sp.GetRequiredService<IEfCoreDbContextTypeProvider>().GetDbContextType(originalDbContextType);
                return sp.GetRequiredService(dbContextType);
            }));

            services.Configure<AbpDbContextOptions>(opts =>
            {
                var dbContextReplacementsProperty = typeof(AbpDbContextOptions).GetProperty("DbContextReplacements", BindingFlags.NonPublic | BindingFlags.Instance);
                var dbContextReplacements = dbContextReplacementsProperty.GetValue(opts, null) as Dictionary<MultiTenantDbContextType, Type>;
                var multiTenantDbContextType = new MultiTenantDbContextType(originalDbContextType, entry.Key.MultiTenancySide);
                dbContextReplacements[multiTenantDbContextType] = targetDbContextType;
            });
        }
        new ChaoEfCoreRepositoryRegistrar(options).AddRepositories();
        return services;
    }
}