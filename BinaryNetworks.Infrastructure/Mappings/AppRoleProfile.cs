using AutoMapper;
using BinaryNetworks.Application.Models.Results.Roles;
using BinaryNetworks.Infrastructure.Identity;

namespace BinaryNetworks.Infrastructure.Mappings;

public class AppRoleProfile : Profile
{
    public AppRoleProfile()
    {
        CreateMap<AppRole, RoleResult>();
    }
}