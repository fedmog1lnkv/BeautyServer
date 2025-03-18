using Api.OptionsSetup;
using Application.Common.Mappings;
using Application.Configurations;
using Domain.Entities;
using Domain.Repositories.Organizations;
using Domain.Repositories.Services;
using Domain.Repositories.Staffs;
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

#endregion
#region Logging

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .WriteTo.Console()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

#endregion

builder.Services.AddAutoMapper(
    config =>
    {
        config.AllowNullCollections = true;
        config.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
        config.AddProfile(new AssemblyMappingProfile(AssemblyReference.Assembly));
    });
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
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
    // db.Database.Migrate();
}

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

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.UseCors("AllowAll");

app.Run();