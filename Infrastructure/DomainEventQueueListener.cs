using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure;

public class DomainEventQueueListener(
    InMemoryDomainEventsQueue queue,
    IServiceScopeFactory scopeFactory,
    ILogger<DomainEventQueueListener> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await foreach (var domainEvent in queue.Reader.ReadAllAsync(cancellationToken))
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var scopedPublisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                await scopedPublisher.Publish(domainEvent, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing domain event");
            }
        }
    }
}