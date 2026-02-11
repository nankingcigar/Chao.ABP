using AutoMapper;
using Volo.Abp.Account;
using Volo.Abp.Identity;

namespace Chao.Abp.Account.Application;

public class ChaoAbpAccountApplicationModuleAutoMapperProfile : Profile
{
    public ChaoAbpAccountApplicationModuleAutoMapperProfile()
    {
        CreateMap<IdentityUser, ProfileDto>()
            .ForMember(dest => dest.HasPassword, op => op.MapFrom(src => src.PasswordHash != null))
            .MapExtraProperties()
            ;
    }
}