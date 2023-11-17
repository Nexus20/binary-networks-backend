using System.IdentityModel.Tokens.Jwt;
using BinaryNetworks.Application.Interfaces.Services;
using BinaryNetworks.Application.Models.Requests.Auth;
using BinaryNetworks.Application.Models.Results.Auth;
using BinaryNetworks.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace BinaryNetworks.Infrastructure.Auth;

public class SignInService : ISignInService {

    private readonly UserManager<AppUser> _userManager;

    private readonly JwtHandler _jwtHandler;

    public SignInService(UserManager<AppUser> userManager, JwtHandler jwtHandler) {
        _userManager = userManager;
        _jwtHandler = jwtHandler;
    }

    public async Task<LoginResult> SignInAsync(LoginRequest request) {
        
        var user = await _userManager.FindByNameAsync(request.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password)) {
            return new LoginResult() {
                ErrorMessage = "Invalid Authentication"
            };
        }

        var signingCredentials = _jwtHandler.GetSigningCredentials();
        var claims = await _jwtHandler.GetClaimsAsync(user);
        var tokenOptions = _jwtHandler.GenerateTokenOptions(signingCredentials, claims);
        var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        
        return new LoginResult() { IsAuthSuccessful = true, Token = token };
    }
}