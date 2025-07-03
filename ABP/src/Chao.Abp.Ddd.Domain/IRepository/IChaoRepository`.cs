using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Chao.Abp.Ddd.Domain.IRepository;

public interface IChaoRepository<TEntity> : IRepository<TEntity>
    where TEntity : class, IEntity
{
    Type ElementType { get; }
    Expression Expression { get; }
    IQueryProvider Provider { get; }

    Task BulkDelete(IList<TEntity> entities);

    Task BulkInsert(IList<TEntity> entities);

    Task BulkUpdate(IList<TEntity> entities);

    IEnumerator<TEntity> GetEnumerator();

    Task<IQueryable<TEntity>> WithDetailsAndAsNoTrackingAsync(params Expression<Func<TEntity, object>>[] propertySelectors);

    IQueryable<TEntity> AsNoTracking(IQueryable<TEntity> entities);
}

public interface IChaoRepository<TEntity, TKey> : IChaoRepository<TEntity>, IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
}