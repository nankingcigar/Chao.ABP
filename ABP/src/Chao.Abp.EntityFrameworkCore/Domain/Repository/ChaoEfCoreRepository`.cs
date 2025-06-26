using Chao.Abp.Ddd.Domain.IRepository;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Chao.Abp.EntityFrameworkCore.Domain.Repository;

public class ChaoEfCoreRepository<TDbContext, TEntity>(IDbContextProvider<TDbContext> dbContextProvider) : EfCoreRepository<TDbContext, TEntity>(dbContextProvider), IChaoRepository<TEntity>
    where TDbContext : IEfCoreDbContext
    where TEntity : class, IEntity
{
    public virtual Type ElementType => QueryableEntity.ElementType;
    public virtual Expression Expression => QueryableEntity.Expression;
    public virtual IQueryProvider Provider => QueryableEntity.Provider;
    protected virtual IQueryable<TEntity> QueryableEntity => AsyncContext.Run(async () => await GetQueryableAsync());
    public virtual ChaoAbpEnittyFrameworkCoreOption ChaoAbpEnittyFrameworkCoreOption => LazyServiceProvider.GetRequiredService<IOptions<ChaoAbpEnittyFrameworkCoreOption>>().Value;

    public virtual async Task BulkDelete(IList<TEntity> entities)
    {
        await (await GetDbContext()).BulkDeleteAsync(entities);
    }

    public virtual async Task BulkInsert(IList<TEntity> entities)
    {
        await (await GetDbContext()).BulkInsertAsync(entities);
    }

    public virtual async Task BulkUpdate(IList<TEntity> entities)
    {
        await (await GetDbContext()).BulkUpdateAsync(entities);
    }

    public virtual async Task<DbContext> GetDbContext()
    {
        return ((await GetDbContextAsync()) as DbContext)!;
    }

    public virtual IEnumerator<TEntity> GetEnumerator()
    {
        return QueryableEntity.GetEnumerator();
    }

    public virtual async Task<IQueryable<TEntity>> WithDetailsAndAsNoTrackingAsync(params Expression<Func<TEntity, object>>[] propertySelectors)
    {
        return (await WithDetailsAsync(propertySelectors)).AsNoTracking();
    }

    public override async Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        if (ChaoAbpEnittyFrameworkCoreOption.EnableUpdateManyOptimization == false)
        {
            await base.UpdateManyAsync(entities, autoSave, cancellationToken);
            return;
        }
        if (entities.Any() == false)
        {
            return;
        }
        string[] basePropertiesName = [];
        var updatedEntities = new ConcurrentBag<TEntity>();
        var dbContext = await GetDbContextAsync();
        Parallel.ForEach(entities, entity =>
        {
            var entityEntry = dbContext.Entry(entity);
            if (entityEntry.Properties.Any(p => p.IsModified == true && ChaoAbpEnittyFrameworkCoreOption.BasicPropertyNames.Contains(p.Metadata.PropertyInfo?.Name ?? string.Empty) == false) == true)
            {
                updatedEntities.Add(entity);
            }
            else
            {
                entityEntry.State = EntityState.Unchanged;
            }
        });
        if (updatedEntities.Any() == false)
        {
            return;
        }
        await base.UpdateManyAsync(updatedEntities.ToList(), autoSave, cancellationToken);
    }
}

public class ChaoEfCoreRepository<TDbContext, TEntity, TKey>(IDbContextProvider<TDbContext> dbContextProvider) : EfCoreRepository<TDbContext, TEntity, TKey>(dbContextProvider), IChaoRepository<TEntity, TKey>
    where TDbContext : IEfCoreDbContext
    where TEntity : class, IEntity<TKey>
{
    public virtual Type ElementType => QueryableEntity.ElementType;
    public virtual Expression Expression => QueryableEntity.Expression;
    public virtual IQueryProvider Provider => QueryableEntity.Provider;
    protected virtual IQueryable<TEntity> QueryableEntity => AsyncContext.Run(async () => await GetQueryableAsync());
    public virtual ChaoAbpEnittyFrameworkCoreOption ChaoAbpEnittyFrameworkCoreOption => LazyServiceProvider.GetRequiredService<IOptions<ChaoAbpEnittyFrameworkCoreOption>>().Value;

    public virtual async Task BulkDelete(IList<TEntity> entities)
    {
        await (await GetDbContext()).BulkDeleteAsync(entities);
    }

    public virtual async Task BulkInsert(IList<TEntity> entities)
    {
        await (await GetDbContext()).BulkInsertAsync(entities);
    }

    public virtual async Task BulkUpdate(IList<TEntity> entities)
    {
        await (await GetDbContext()).BulkUpdateAsync(entities);
    }

    public virtual async Task<DbContext> GetDbContext()
    {
        return ((await GetDbContextAsync()) as DbContext)!;
    }

    public virtual IEnumerator<TEntity> GetEnumerator()
    {
        return QueryableEntity.GetEnumerator();
    }

    public virtual async Task<IQueryable<TEntity>> WithDetailsAndAsNoTrackingAsync(params Expression<Func<TEntity, object>>[] propertySelectors)
    {
        return (await WithDetailsAsync(propertySelectors)).AsNoTracking();
    }

    public override async Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        if (ChaoAbpEnittyFrameworkCoreOption.EnableUpdateManyOptimization == false)
        {
            await base.UpdateManyAsync(entities, autoSave, cancellationToken);
            return;
        }
        if (entities.Any() == false)
        {
            return;
        }
        string[] basePropertiesName = [];
        var updatedEntities = new ConcurrentBag<TEntity>();
        var dbContext = await GetDbContextAsync();
        Parallel.ForEach(entities, entity =>
        {
            var entityEntry = dbContext.Entry(entity);
            if (entityEntry.Properties.Any(p => p.IsModified == true && ChaoAbpEnittyFrameworkCoreOption.BasicPropertyNames.Contains(p.Metadata.PropertyInfo?.Name ?? string.Empty) == false) == true)
            {
                updatedEntities.Add(entity);
            }
            else
            {
                entityEntry.State = EntityState.Unchanged;
            }
        });
        if (updatedEntities.Any() == false)
        {
            return;
        }
        await base.UpdateManyAsync(updatedEntities.ToList(), autoSave, cancellationToken);
    }
}