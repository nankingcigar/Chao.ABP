using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.Auditing;
using Volo.Abp.AuditLogging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Json;

namespace Chao.Abp.AuditLogging.Domain;

[Dependency(ReplaceServices = true)]
public class ChaoAuditLogInfoToAuditLogConverter(IGuidGenerator guidGenerator, IExceptionToErrorInfoConverter exceptionToErrorInfoConverter, IJsonSerializer jsonSerializer, IOptions<AbpExceptionHandlingOptions> exceptionHandlingOptions, AuditLogEntityTypeFullNameConverter auditLogEntityTypeFullNameConverter) : AuditLogInfoToAuditLogConverter(guidGenerator, exceptionToErrorInfoConverter, jsonSerializer, exceptionHandlingOptions, auditLogEntityTypeFullNameConverter)
{
    public override async Task<AuditLog> ConvertAsync(AuditLogInfo auditInfo)
    {
        foreach (var entityChange in auditInfo.EntityChanges)
        {
            entityChange.EntityId = entityChange.EntityId.Truncate(EntityChangeConsts.MaxEntityIdLength);
        }
        var auditLog = await base.ConvertAsync(auditInfo);
        return auditLog;
    }
}