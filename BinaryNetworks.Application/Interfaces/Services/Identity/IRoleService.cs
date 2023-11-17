using BinaryNetworks.Application.Models.Requests.Roles;
using BinaryNetworks.Application.Models.Results.Roles;

namespace BinaryNetworks.Application.Interfaces.Services.Identity;

public interface IRoleService
{
    Task<List<RoleResult>> GetAsync(CancellationToken cancellationToken = default);
    Task<RoleResult> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<RoleResult> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);
}