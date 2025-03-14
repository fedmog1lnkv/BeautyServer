using Application.Abstractions;
using Application.Common.Behaviors;
using FluentValidation;
using MediatR.NotificationPublishers;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            cfg.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
            cfg.AddOpenBehavior(typeof(LoggingPipelineBehavior<,>));
            cfg.AddOpenBehavior(typeof(UnitOfWorkPipelineBehavior<,>));

            cfg.NotificationPublisher = new TaskWhenAllPublisher();
            cfg.NotificationPublisherType = typeof(TaskWhenAllPublisher);
        });

        services.AddValidatorsFromAssemblies(
            new[] { Assembly.GetExecutingAssembly() },
            includeInternalTypes: true);

        return services;
    }
}
