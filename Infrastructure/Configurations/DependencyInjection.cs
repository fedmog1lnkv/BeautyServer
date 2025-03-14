using Application.Abstractions;
using Domain.Repositories;
using Domain.Repositories.Organizations;
using Domain.Repositories.PhoneChallenges;
using Domain.Repositories.Staffs;
using Domain.Repositories.Users;
using Domain.Repositories.Venues;
using Infrastructure.Authentication;
using Infrastructure.Repositories.Organizations;
using Infrastructure.Repositories.PhoneChallenges;
using Infrastructure.Repositories.Staffs;
using Infrastructure.Repositories.Users;
using Infrastructure.Repositories.Venues;
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

        // User
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserReadOnlyRepository, UserReadOnlyRepository>();

        // Phone challenge
        services.AddScoped<IPhoneChallengeRepository, PhoneChallengeRepository>();

        // Organization
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IOrganizationReadOnlyRepository, OrganizationReadOnlyRepository>();

        // Venue
        services.AddScoped<IVenueReadOnlyRepository, VenueReadOnlyRepository>();

        // Staff
        services.AddScoped<IStaffReadOnlyRepository, StaffReadOnlyRepository>();


        return services;
    }
}