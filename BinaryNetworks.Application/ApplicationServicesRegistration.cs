using System.Reflection;
using BinaryNetworks.Application.Interfaces.Services.BinaryNetworks;
using BinaryNetworks.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BinaryNetworks.Application;

public static class ApplicationServicesRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        services.AddScoped<IBinaryNetworksService, BinaryNetworksService>();

        return services;
    }
}