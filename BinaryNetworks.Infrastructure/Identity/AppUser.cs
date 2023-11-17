using BinaryNetworks.Domain.Entities.Abstract;
using Microsoft.AspNetCore.Identity;

namespace BinaryNetworks.Infrastructure.Identity;

public class AppUser : IdentityUser, ITimeMarkedEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public virtual List<AppUserRole> UserRoles { get; set; }
}