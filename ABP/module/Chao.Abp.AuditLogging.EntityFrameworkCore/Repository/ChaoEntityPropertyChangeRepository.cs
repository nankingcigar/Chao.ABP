using Chao.Abp.AuditLogging.Domain;
using Chao.Abp.EntityFrameworkCore.Domain.Repository;
using System;
using Volo.Abp.AuditLogging;
using Volo.Abp.EntityFrameworkCore;

namespace Chao.Abp.AuditLogging.EntityFrameworkCore.Repository;

public class ChaoEntityPropertyChangeRepository(IDbContextProvider<ChaoAbpAuditLoggingDbContext> dbContextProvider) : ChaoEfCoreRepository<ChaoAbpAuditLoggingDbContext, EntityPropertyChange, Guid>(dbContextProvider), IChaoEntityPropertyChangeRepository
{
}