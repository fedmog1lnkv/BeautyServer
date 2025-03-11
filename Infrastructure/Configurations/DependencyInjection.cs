using Application.Abstractions;
using Domain.Repositories;
using Infrastructure.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(
            options =>
                options.UseNpgsql(configuration.GetConnectionString("DatabaseConnectionString")));

        services.AddTransient<IJwtProvider, JwtProvider>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}