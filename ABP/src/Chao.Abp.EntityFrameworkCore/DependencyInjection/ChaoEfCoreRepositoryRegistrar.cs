using Chao.Abp.EntityFrameworkCore.Domain.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;

namespace Chao.Abp.EntityFrameworkCore.DependencyInjection;

public class ChaoEfCoreRepositoryRegistrar : EfCoreRepositoryRegistrar
{
    public ChaoEfCoreRepositoryRegistrar(AbpDbContextRegistrationOptions options)
        : base(options)
    {
    }

    protected override Type GetRepositoryType(Type dbContextType, Type entityType)
    {
        return typeof(ChaoEfCoreRepository<,>).MakeGenericType(dbContextType, entityType);
    }

    protected override Type GetRepositoryType(Type dbContextType, Type entityType, Type primaryKeyType)
    {
        return typeof(ChaoEfCoreRepository<,,>).MakeGenericType(dbContextType, entityType, primaryKeyType);
    }

    protected override void RegisterDefaultRepository(Type entityType)
    {
        var implementationType = GetDefaultRepositoryImplementationType(entityType);
        Options.Services.AddDefaultRepository(
            entityType,
            implementationType
        );
        Options.Services.AddChaoRepository(
            entityType,
            implementationType
        );
    }
}