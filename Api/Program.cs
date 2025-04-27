using Api.BackgroundServices;
using Api.Hubs.RecordChat;
using Api.OptionsSetup;
using Api.Storages;
using Api.Utils;
using Application.Abstractions;
using Application.Common.Mappings;
using Application.Configurations;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

# region DI

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureOptions<JwtOptionsSetup>();
builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();
builder.Services.Configure<TwoFaSettings>(builder.Configuration.GetSection("2fa"));
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddSignalR();
foreach (var mapping in HubClientMapping.EventToHubClientMap)
{
    var eventType = mapping.Key;
    var hubClientType = mapping.Value;

    var interfaceType = typeof(IHubClient<>).MakeGenericType(eventType);
    var implementationType = hubClientType;

    builder.Services.AddScoped(interfaceType, implementationType);
}

builder.Services.AddSingleton<NotificationSchedulerStorage>();
builder.Services.AddHostedService<RecordNotificationScheldueStorageBackgroundService>();
builder.Services.AddHostedService<RecordReminderBackgroundService>();

#endregion
#region Logging

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
    .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy/MM/dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .Enrich.WithProperty("Timestamp", DateTime.UtcNow)
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

#endregion
#region Mapper

builder.Services.AddAutoMapper(
    config =>
    {
        config.AllowNullCollections = true;
        config.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
        config.AddProfile(new AssemblyMappingProfile(AssemblyReference.Assembly));
    });

#endregion
#region Cors

builder.Services.AddCors(
    options =>
    {
        options.AddPolicy(
            "AllowAll",
            builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
    });

#endregion

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // db.Database.EnsureDeleted();
    // db.Database.EnsureCreated();
    db.Database.Migrate();
}

#region Test

// using (var scope = app.Services.CreateScope())
// {
//     var staffReadOnlyRepository = scope.ServiceProvider.GetRequiredService<IStaffReadOnlyRepository>();
//     var staffRepository = scope.ServiceProvider.GetRequiredService<IStaffRepository>();
//     var organizationRepo = scope.ServiceProvider.GetRequiredService<IOrganizationRepository>();
//     var organizationReadOnlyRepo = scope.ServiceProvider.GetRequiredService<IOrganizationReadOnlyRepository>();
//     var serviceRepo = scope.ServiceProvider.GetRequiredService<IServiceRepository>();
//     var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//
//     var orgId = Guid.NewGuid();
//     var organization = Organization.Create(
//         orgId,
//         "test org 1",
//         "#FFFFFF");
//     organizationRepo.Add(organization.Value);
//     await dbContext.SaveChangesAsync();
//
//     var serviceId = Guid.NewGuid();
//     var service = await Service.Create(serviceId, orgId, "service 1", organizationReadOnlyRepo);
//     serviceRepo.Add(service.Value);
//     await dbContext.SaveChangesAsync();
//
//     var staffId = Guid.NewGuid();
//     var staff = await Staff.Create(
//         staffId,
//         orgId,
//         "staffchik",
//         "7904122342",
//         staffReadOnlyRepository,
//         organizationReadOnlyRepo);
//     staffRepository.Add(staff.Value);
//     await dbContext.SaveChangesAsync();
//
//     var staffFromDb = await staffReadOnlyRepository.GetByIdWithServices(staffId);
//     
//     staffFromDb?.AddService(service.Value);
//     await dbContext.SaveChangesAsync();
//
//     staffFromDb = await staffReadOnlyRepository.GetByIdWithServices(staffId);
// }

#endregion



//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<RecordChatHub>("/record_chat");
});

app.UseCors("AllowAll");

app.Run();