using BinaryNetworks.Domain.Entities.Abstract;
using Microsoft.AspNetCore.Identity;

namespace BinaryNetworks.Infrastructure.Identity;

public class AppUserRole : IdentityUserRole<string>, ITimeMarkedEntity {

    public virtual AppUser User { get; set; }

    public virtual AppRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}