using DAL.Context;
using DAL.Interfaces;
using DAL.Models;
using DAL.PostgresRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DAL;

public static class ConfigurationExtensions
{
    public static void ConfigureDAL(this IServiceCollection services, string connectionString)
    {
        services.AddIdentityCore<User>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
        
        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IEventRepository, EventPostgres>();
        services.AddScoped<IUserRepository, UserPostgres>();
        services.AddScoped<IEventParticipantsRepository, EventParticipantsPostgres>();
    }
}