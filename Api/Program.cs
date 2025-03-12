using Api.OptionsSetup;
using Application.Common.Mappings;
using Application.Configurations;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Configurations;
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
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
}

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