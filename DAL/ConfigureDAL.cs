using DAL.Context;
using DAL.Interfaces;
using DAL.Models;
using DAL.PostgresRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DAL;

public static class ConfigurationExtensions
{
    public static void ConfigureDAL(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

        services.AddIdentity<User, IdentityRole<int>>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 6;
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
        
        services.AddScoped<IEventRepository, EventPostgres>();
        services.AddScoped<IUserRepository, UserPostgres>();
        services.AddScoped<IEventParticipantsRepository, EventParticipantsPostgres>();
    }
}