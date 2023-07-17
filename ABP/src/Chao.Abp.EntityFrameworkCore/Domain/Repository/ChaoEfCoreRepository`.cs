/*
 * @Author: Chao Yang
 * @Date: 2020-11-16 17:20:40
 * @LastEditor: Chao Yang
 * @LastEditTime: 2020-11-29 03:36:09
 */

using Chao.Abp.Ddd.Domain.IRepository;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Chao.Abp.EntityFrameworkCore.Domain.Repository;

public class ChaoEfCoreRepository<TDbContext, TEntity> : EfCoreRepository<TDbContext, TEntity>, IChaoRepository<TEntity>
    where TDbContext : IEfCoreDbContext
    where TEntity : class, IEntity
{
    public ChaoEfCoreRepository(IDbContextProvider<TDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public virtual Type ElementType => QueryableEntity.ElementType;
    public virtual Expression Expression => QueryableEntity.Expression;
    public virtual IQueryProvider Provider => QueryableEntity.Provider;
    protected virtual IQueryable<TEntity> QueryableEntity => AsyncContext.Run(async () => await GetQueryableAsync());

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

    public void DisableAutoDetectChangesEnabled()
    {
        throw new NotImplementedException();
    }

    public virtual async Task<DbContext> GetDbContext()
    {
        return (await GetDbContextAsync()) as DbContext;
    }

    public virtual IEnumerator<TEntity> GetEnumerator()
    {
        return QueryableEntity.GetEnumerator();
    }

    public virtual async Task<IQueryable<TEntity>> WithDetailsAndAsNoTrackingAsync(params Expression<Func<TEntity, object>>[] propertySelectors)
    {
        return (await WithDetailsAsync(propertySelectors)).AsNoTracking();
    }
}

public class ChaoEfCoreRepository<TDbContext, TEntity, TKey> : EfCoreRepository<TDbContext, TEntity, TKey>, IChaoRepository<TEntity, TKey>
    where TDbContext : IEfCoreDbContext
    where TEntity : class, IEntity<TKey>
{
    public ChaoEfCoreRepository(IDbContextProvider<TDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public virtual Type ElementType => QueryableEntity.ElementType;
    public virtual Expression Expression => QueryableEntity.Expression;
    public virtual IQueryProvider Provider => QueryableEntity.Provider;
    protected virtual IQueryable<TEntity> QueryableEntity => AsyncContext.Run(async () => await GetQueryableAsync());

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
        return (await GetDbContextAsync()) as DbContext;
    }

    public virtual IEnumerator<TEntity> GetEnumerator()
    {
        return QueryableEntity.GetEnumerator();
    }

    public virtual async Task<IQueryable<TEntity>> WithDetailsAndAsNoTrackingAsync(params Expression<Func<TEntity, object>>[] propertySelectors)
    {
        return (await WithDetailsAsync(propertySelectors)).AsNoTracking();
    }
}