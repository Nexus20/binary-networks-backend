using BinaryNetworks.Application.Models.Results.Abstract;
using BinaryNetworks.Application.Models.Results.Roles;

namespace BinaryNetworks.Application.Models.Results.Users;

public class UserResult : BaseResult
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public List<RoleResult> Roles { get; set; } = null!;
    public string SelectedCulture { get; set; } = null!;
}