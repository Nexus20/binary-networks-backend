﻿using AutoMapper;
using BinaryNetworks.Application.Exceptions;
using BinaryNetworks.Application.Interfaces.Services.Identity;
using BinaryNetworks.Application.Models.Requests.Users;
using BinaryNetworks.Application.Models.Results.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BinaryNetworks.Infrastructure.Identity.Services;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IMapper _mapper;

    public UserService(UserManager<AppUser> userManager, IMapper mapper, RoleManager<AppRole> roleManager)
    {
        _userManager = userManager;
        _mapper = mapper;
        _roleManager = roleManager;
    }

    public async Task<List<UserResult>> GetAsync(CancellationToken cancellationToken = default)
    {
        var source = await _userManager.Users.ToListAsync(cancellationToken: cancellationToken);

        var result = _mapper.Map<List<AppUser>, List<UserResult>>(source);
        return result;
    }

    public async Task<UserResult> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var source = await _userManager.FindByIdAsync(id);

        if(source == null)
            throw new NotFoundException(nameof(AppUser), nameof(id));
        
        var result = _mapper.Map<AppUser, UserResult>(source);
        return result;
    }

    public async Task<UserResult> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var userExists =
            await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken: cancellationToken);

        if (userExists)
            throw new ValidationException($"{nameof(AppUser)} with such email \"{request.Email}\" already exists");
        
        var userToCreate = _mapper.Map<CreateUserRequest, AppUser>(request);

        await _userManager.CreateAsync(userToCreate);

        var roles = await _roleManager.Roles.Where(x => request.RolesIds.Contains(x.Id)).ToListAsync(cancellationToken);

        if (roles.Count < request.RolesIds.Count)
            throw new ValidationException("One or more of roles ids are invalid");

        var rolesNames = roles.Select(x => x.Name);

        await _userManager.AddToRolesAsync(userToCreate, rolesNames);

        var result = _mapper.Map<AppUser, UserResult>(userToCreate);

        return result;
    }

    public async Task<UserResult> UpdateAsync(UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var userToUpdate = await _userManager.FindByIdAsync(request.Id);
        
        if (userToUpdate == null)
            throw new NotFoundException(nameof(AppUser), nameof(request.Id));
        
        _mapper.Map(request, userToUpdate);
        
        await _userManager.UpdateAsync(userToUpdate);
        
        var updatedUser = await _userManager.FindByIdAsync(request.Id);
        
        var result = _mapper.Map<AppUser, UserResult>(updatedUser);
        
        return result;
    }
    
    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var userToDelete = await _userManager.FindByIdAsync(id);

        if (userToDelete == null)
            throw new NotFoundException(nameof(AppUser), nameof(id));
        
        if(userToDelete.UserName == "root@wine-quality.com")
            throw new ValidationException("You can't delete root user");

        await _userManager.DeleteAsync(userToDelete);
    }
}