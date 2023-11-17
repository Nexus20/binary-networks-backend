using System.ComponentModel.DataAnnotations;

namespace BinaryNetworks.Application.Models.Requests.Roles;

public class CreateRoleRequest
{
    [Required] public string Name { get; set; } = null!;
}