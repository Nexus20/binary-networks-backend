using System.Reflection;
using Azure.Storage.Blobs;
using BinaryNetworks.Application.Interfaces.FileStorage;
using BinaryNetworks.Application.Interfaces.Persistence;
using BinaryNetworks.Application.Interfaces.Services;
using BinaryNetworks.Application.Interfaces.Services.Identity;
using BinaryNetworks.Infrastructure.Auth;
using BinaryNetworks.Infrastructure.FileStorage;
using BinaryNetworks.Infrastructure.Identity;
using BinaryNetworks.Infrastructure.Identity.Services;
using BinaryNetworks.Infrastructure.Persistence;
using BinaryNetworks.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BinaryNetworks.Infrastructure;

public static class InfrastructureServicesRegistration
{

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddIdentity<AppUser, AppRole>()
            .AddUserStore<UserStore<AppUser, AppRole, ApplicationDbContext, string, IdentityUserClaim<string>, AppUserRole,
                IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>>()
            .AddRoleStore<RoleStore<AppRole, ApplicationDbContext, string, AppUserRole, IdentityRoleClaim<string>>>()
            .AddSignInManager<SignInManager<AppUser>>()
            .AddRoleManager<RoleManager<AppRole>>()
            .AddUserManager<UserManager<AppUser>>();
        
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        
        // services.AddScoped<IIdentityInitializer, IdentityInitializer>();
        services.AddScoped<ISignInService, SignInService>();
        services.AddScoped<JwtHandler>();

        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserService, UserService>();
        
        var blobStorageConnectionString = configuration.GetValue<string>("BlobStorageSettings:ConnectionString");
        services.AddSingleton(_ => new BlobServiceClient(blobStorageConnectionString));
        services.AddScoped<IFileStorageService, BlobStorageService>();

        return services;
    }
}