using Application.Abstractions;
using Domain.Repositories;
using Domain.Repositories.PhoneChallenges;
using Domain.Repositories.Users;
using Infrastructure.Authentication;
using Infrastructure.Repositories.PhoneChallenge;
using Infrastructure.Repositories.User;
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
        services.AddDbContextFactory<ApplicationDbContext>(
            options => options
                .UseNpgsql(configuration.GetConnectionString("DatabaseConnectionString") ?? string.Empty));

        services.AddTransient<IJwtProvider, JwtProvider>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserReadOnlyRepository, UserReadOnlyRepository>();
        services.AddScoped<IPhoneChallengeRepository, PhoneChallengeRepository>();

        return services;
    }
}