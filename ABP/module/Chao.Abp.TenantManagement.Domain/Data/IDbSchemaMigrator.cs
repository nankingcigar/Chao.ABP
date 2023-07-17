using System.Threading.Tasks;

namespace Chao.Abp.TenantManagement.Domain.Data;

public interface IDbSchemaMigrator
{
    Task MigrateAsync();
}