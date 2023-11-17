using BinaryNetworks.Application.Models.Requests.Auth;
using BinaryNetworks.Application.Models.Results.Auth;

namespace BinaryNetworks.Application.Interfaces.Services;

public interface ISignInService {

    Task<LoginResult> SignInAsync(LoginRequest request);
}