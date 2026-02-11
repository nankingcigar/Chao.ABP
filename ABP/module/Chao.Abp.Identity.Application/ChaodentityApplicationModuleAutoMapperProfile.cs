using AutoMapper;
using Volo.Abp.Identity;

namespace Chao.Abp.Identity.Application;

public class ChaodentityApplicationModuleAutoMapperProfile : Profile
{
    public ChaodentityApplicationModuleAutoMapperProfile()
    {
        CreateMap<IdentityUser, IdentityUserDto>()
            .MapExtraProperties()
            ;
        CreateMap<IdentityRole, IdentityRoleDto>()
            .MapExtraProperties()
            ;
    }
}