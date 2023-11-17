using BinaryNetworks.Domain.Entities.Abstract;
using Microsoft.AspNetCore.Identity;

namespace BinaryNetworks.Infrastructure.Identity;

public class AppRole : IdentityRole, ITimeMarkedEntity
{
    public AppRole()
    {
        
    }
    
    public AppRole(string roleName) : base(roleName) { }
    
    public virtual List<AppUserRole> UserRoles { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}