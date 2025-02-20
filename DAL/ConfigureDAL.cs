using DAL.Context;
using DAL.Interfaces;
using DAL.PostgresReposistory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DAL;

public static class ConfigurationExtensions
{
    public static void ConfigureDAL(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IEventRepository, EventPostgres>();
    }
}