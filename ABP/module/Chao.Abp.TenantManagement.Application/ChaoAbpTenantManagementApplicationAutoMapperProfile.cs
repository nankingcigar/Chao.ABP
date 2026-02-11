using AutoMapper;
using Volo.Abp.TenantManagement;

namespace Chao.Abp.TenantManagement.Application;

public class ChaoAbpTenantManagementApplicationAutoMapperProfile : Profile
{
    public ChaoAbpTenantManagementApplicationAutoMapperProfile()
    {
        CreateMap<Tenant, TenantDto>()
            .MapExtraProperties()
            ;
    }
}