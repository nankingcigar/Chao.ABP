using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Chao.Abp.TenantManagement.Domain.Data;

public class NullDbSchemaMigrator : IDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}