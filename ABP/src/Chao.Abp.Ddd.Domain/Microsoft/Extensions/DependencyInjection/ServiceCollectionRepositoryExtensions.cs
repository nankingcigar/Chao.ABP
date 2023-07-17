using Chao.Abp.Ddd.Domain.IRepository;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using Volo.Abp.Domain.Entities;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionRepositoryExtensions
{
    public static IServiceCollection AddChaoRepository(
           this IServiceCollection services,
           Type entityType,
           Type repositoryImplementationType,
           bool replaceExisting = false)
    {
        var repositoryInterface = typeof(IChaoRepository<>).MakeGenericType(entityType);
        if (repositoryInterface.IsAssignableFrom(repositoryImplementationType))
        {
            RegisterService(services, repositoryInterface, repositoryImplementationType, replaceExisting);
        }
        var primaryKeyType = EntityHelper.FindPrimaryKeyType(entityType);
        if (primaryKeyType != null)
        {
            var repositoryInterfaceWithPk = typeof(IChaoRepository<,>).MakeGenericType(entityType, primaryKeyType);
            if (repositoryInterfaceWithPk.IsAssignableFrom(repositoryImplementationType))
            {
                RegisterService(services, repositoryInterfaceWithPk, repositoryImplementationType, replaceExisting);
            }
        }

        return services;
    }

    private static void RegisterService(
        IServiceCollection services,
        Type serviceType,
        Type implementationType,
        bool replaceExisting)
    {
        if (replaceExisting)
        {
            services.Replace(ServiceDescriptor.Transient(serviceType, implementationType));
        }
        else
        {
            services.TryAddTransient(serviceType, implementationType);
        }
    }
}