using Chao.Abp.Ddd.Domain.IRepository;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Volo.Abp.Domain.Repositories;

public static class RepositoryExtension
{
    public static bool Any<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate) where TEntity : class, IEntity

    {
        return AsyncContext.Run(async () =>
        {
            return (await repository.GetQueryableAsync()).Any(predicate);
        });
    }

    public static async Task BulkDelete<TEntity>(this IRepository<TEntity> repository, IList<TEntity> entities) where TEntity : class, IEntity
    {
        if (repository is IChaoRepository<TEntity> chaoRepository)
        {
            await chaoRepository.BulkDelete(entities);
            return;
        }
        throw new ArgumentException("Not IChaoRepository");
    }

    public static async Task BulkInsert<TEntity>(this IRepository<TEntity> repository, IList<TEntity> entities) where TEntity : class, IEntity
    {
        if (repository is IChaoRepository<TEntity> chaoRepository)
        {
            await chaoRepository.BulkInsert(entities);
            return;
        }
        throw new ArgumentException("Not IChaoRepository");
    }

    public static async Task BulkUpdate<TEntity>(this IRepository<TEntity> repository, IList<TEntity> entities) where TEntity : class, IEntity
    {
        if (repository is IChaoRepository<TEntity> chaoRepository)
        {
            await chaoRepository.BulkUpdate(entities);
            return;
        }
        throw new ArgumentException("Not IChaoRepository");
    }

    public static async Task<TEntity> InsertAsync<TEntity>(this IRepository<IEntity> repository, TEntity entity, CancellationToken cancellationToken = default) where TEntity : class, IEntity
    {
        if (repository is IChaoRepository<TEntity> chaoRepository)
        {
            return await chaoRepository.InsertAsync(entity, false, cancellationToken);
        }
        throw new ArgumentException("Not IChaoRepository");
    }

    public static async Task InsertDataFromJsonFile<TEntity>(this IRepository<TEntity> repository, string jsonFilePath)
                where TEntity : class, IEntity
    {
        var entitesText = File.ReadAllText(jsonFilePath);
        var entites = JsonSerializer.Deserialize<List<TEntity>>(entitesText, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        await repository.InsertManyAsync(entites!);
    }

    public static IEnumerable<TResult> Select<TEntity, TResult>(this IRepository<TEntity> repository, Func<TEntity, TResult> selector) where TEntity : class, IEntity

    {
        return AsyncContext.Run(async () =>
        {
            return (await repository.GetQueryableAsync()).Select(selector);
        });
    }

    public static IQueryable<TEntity> Where<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate) where TEntity : class, IEntity

    {
        return AsyncContext.Run(async () =>
        {
            return (await repository.GetQueryableAsync()).Where(predicate);
        });
    }

    public static async Task<IQueryable<TEntity>> WithDetailsAndAsNoTrackingAsync<TEntity>(this IRepository<TEntity> repository, params Expression<Func<TEntity, object>>[] propertySelectors) where TEntity : class, IEntity
    {
        if (repository is IChaoRepository<TEntity> chaoRepository)
        {
            return await chaoRepository.WithDetailsAndAsNoTrackingAsync(propertySelectors);
        }
        throw new ArgumentException("Not IChaoRepository");
    }

    public static IQueryable<TEntity> AsNoTracking<TEntity>(this IRepository<TEntity> repository, IQueryable<TEntity> entities) where TEntity : class, IEntity
    {
        if (repository is IChaoRepository<TEntity> chaoRepository)
        {
            return chaoRepository.AsNoTracking(entities);
        }
        throw new ArgumentException("Not IChaoRepository");
    }
}