using AutoMapper;
using BinaryNetworks.Application.Models.Requests.Users;
using BinaryNetworks.Application.Models.Results.Users;
using BinaryNetworks.Infrastructure.Identity;

namespace BinaryNetworks.Infrastructure.Mappings;

public class AppUserProfile : Profile
{
    public AppUserProfile()
    {
        CreateMap<AppUser, UserResult>()
            .ForMember(d => d.Roles, o => o.MapFrom(s => s.UserRoles.Select(ur => ur.Role)))
            .ForMember(d => d.Phone, o => o.MapFrom(s => s.PhoneNumber));
        CreateMap<CreateUserRequest, AppUser>()
            .ForMember(d => d.UserName, o => o.MapFrom(s => s.Email))
            .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.Phone));
        CreateMap<UpdateUserRequest, AppUser>()
            .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.Phone));
    }
}