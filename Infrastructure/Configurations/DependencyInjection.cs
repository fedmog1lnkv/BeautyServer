using Amazon.Runtime;
using Amazon.S3;
using Application.Abstractions;
using Domain.Repositories;
using Domain.Repositories.Organizations;
using Domain.Repositories.PhoneChallenges;
using Domain.Repositories.Records;
using Domain.Repositories.Services;
using Domain.Repositories.Staffs;
using Domain.Repositories.Users;
using Domain.Repositories.Utils;
using Domain.Repositories.Venues;
using Infrastructure.Authentication.Staff;
using Infrastructure.Authentication.User;
using Infrastructure.Repositories.Organizations;
using Infrastructure.Repositories.PhoneChallenges;
using Infrastructure.Repositories.Records;
using Infrastructure.Repositories.Services;
using Infrastructure.Repositories.Staffs;
using Infrastructure.Repositories.Users;
using Infrastructure.Repositories.Utils;
using Infrastructure.Repositories.Venues;
using Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContextFactory<ApplicationDbContext>(
            (serviceProvider, options) =>
            {
                var environment = serviceProvider.GetRequiredService<IHostEnvironment>();

                options.UseNpgsql(configuration.GetConnectionString("DatabaseConnectionString") ?? string.Empty);

                if (environment.IsDevelopment())
                    options.EnableSensitiveDataLogging();
            });
        
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var accessKey = configuration["AWS:AccessKey"];
            var secretKey = configuration["AWS:SecretKey"];
            var region = Amazon.RegionEndpoint.GetBySystemName(configuration["AWS:Region"]);
            var endpointUrl = configuration["AWS:EndpointUrl"];

            var s3Config = new AmazonS3Config
            {
                RegionEndpoint = region,
                ServiceURL = endpointUrl,
                ForcePathStyle = true
            };

            var credentials = new BasicAWSCredentials(accessKey, secretKey);

            return new AmazonS3Client(credentials, s3Config);
        });
        
        services.AddScoped<S3StorageUtils>();

        services.AddTransient<IUserJwtProvider, UserJwtProvider>();
        services.AddTransient<IStaffJwtProvider, StaffJwtProvider>();
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
        services.AddScoped<IVenueRepository, VenueRepository>();
        services.AddScoped<IVenueReadOnlyRepository, VenueReadOnlyRepository>();

        // Staff
        services.AddScoped<IStaffRepository, StaffRepository>();
        services.AddScoped<IStaffReadOnlyRepository, StaffReadOnlyRepository>();

        // Service
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IServiceReadOnlyRepository, ServiceReadOnlyRepository>();

        // Record
        services.AddScoped<IRecordRepository, RecordRepository>();
        services.AddScoped<IRecordReadOnlyRepository, RecordReadOnlyRepository>();
        
        // Utils
        services.AddScoped<ILocationRepository, LocationRepository>();

        services.AddSingleton<InMemoryDomainEventsQueue>();
        services.AddSingleton<IDomainEventBus, DomainEventBus>();
        services.AddHostedService<DomainEventQueueListener>();

        return services;
    }
}