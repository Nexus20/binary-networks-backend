using System.ComponentModel.DataAnnotations;

namespace BinaryNetworks.Application.Models.Requests.Users;

public class CreateUserRequest
{
    [Required] public string FirstName { get; set; } = null!;
    [Required] public string LastName { get; set; } = null!;
    [Required] public string Email { get; set; } = null!;
    [Required] public string Phone { get; set; } = null!;
    [Required] public string Password { get; set; } = null!;
    [Required] public List<string> RolesIds { get; set; } = null!;
}