using System.Reflection;
using BLL.DTO;
using BLL.Interfaces;
using BLL.Mapper;
using BLL.Services;
using BLL.Validators;
using DAL;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace BLL;

public static class ConfigurationExtensions
{
    public static void ConfigureBLL(this IServiceCollection services, string connectionString)
    {
        services.ConfigureDAL(connectionString);
        
        var config = TypeAdapterConfig.GlobalSettings;
        
        config.Scan(Assembly.GetAssembly(typeof(EventConfigMapper))!);
        config.Scan(Assembly.GetAssembly(typeof(EventParticipantsMapper))!);
        
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
        
        services.AddTransient<IEventServices, EventServices>();
        services.AddTransient<IUserServices, UserServices>();
        services.AddTransient<IValidator<EventDTO>, EventValidator>();
    }
}