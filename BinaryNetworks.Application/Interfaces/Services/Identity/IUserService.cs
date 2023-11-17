using BinaryNetworks.Application.Models.Requests.Users;
using BinaryNetworks.Application.Models.Results.Users;

namespace BinaryNetworks.Application.Interfaces.Services.Identity;

public interface IUserService
{
    Task<List<UserResult>> GetAsync(CancellationToken cancellationToken = default);
    Task<UserResult> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<UserResult> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<UserResult> UpdateAsync(UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}