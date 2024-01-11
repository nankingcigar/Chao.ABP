using Microsoft.Extensions.Options;
using System;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Chao.Abp.MultiTenancy;

public class ChaoCurrentTenant(ICurrentTenantAccessor currentTenantAccessor) : ICurrentTenant, ITransientDependency
{
    private readonly ICurrentTenantAccessor _currentTenantAccessor = currentTenantAccessor;

    public virtual ChaoAbpMultiTenancyOptions ChaoAbpMultiTenancyOptions => ChaoAbpMultiTenancyOptionsOptions!.Value;
    public virtual IOptions<ChaoAbpMultiTenancyOptions>? ChaoAbpMultiTenancyOptionsOptions { get; set; }
    public virtual Guid? Id => _currentTenantAccessor.Current!.TenantId;
    public virtual bool IsAvailable => Id.HasValue;
    public virtual string? Name => GetName();

    public IDisposable Change(Guid? id, string? name = null)
    {
        return SetCurrent(id, name);
    }

    public virtual string? GetName()
    {
        var name = _currentTenantAccessor.Current!.Name;
        if (string.IsNullOrWhiteSpace(name) == true)
        {
            return ChaoAbpMultiTenancyOptions.DefaultTenantName;
        }
        return name;
    }

    private IDisposable SetCurrent(Guid? tenantId, string? name = null)
    {
        var parentScope = _currentTenantAccessor.Current;
        _currentTenantAccessor.Current = new BasicTenantInfo(tenantId, name);
        return new DisposeAction<ValueTuple<ICurrentTenantAccessor, BasicTenantInfo?>>(static (state) =>
        {
            var (currentTenantAccessor, parentScope) = state;
            currentTenantAccessor.Current = parentScope;
        }, (_currentTenantAccessor, parentScope));
    }
}