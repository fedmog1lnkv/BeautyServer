using Domain.Primitives;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure;

public class IntegrationEventQueueListener(
    InMemoryIntegrationEventsQueue queue,
    IPublisher publisher,
    IServiceProvider serviceProvider)
    : IHostedService, IDisposable
{
    private readonly IPublisher _publisher = publisher;
    private Task? _processingTask;
    private CancellationTokenSource? _cts;

    public async Task SendAsync<T>(T integrationEvent, CancellationToken cancellationToken = default)
        where T : class, IIntegrationEvent
    {
        await queue.Writer.WriteAsync(integrationEvent, cancellationToken);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _processingTask = ProcessEventsAsync(_cts.Token);
        return Task.CompletedTask;
    }

    private async Task ProcessEventsAsync(CancellationToken cancellationToken)
    {
        await foreach (var integrationEvent in queue.Reader.ReadAllAsync(cancellationToken))
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var scopedPublisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                try
                {
                    await scopedPublisher.Publish(integrationEvent, cancellationToken);
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<IntegrationEventQueueListener>>();
                    logger.LogError(ex, "Error processing integration event");
                }
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts?.Cancel();
        return _processingTask ?? Task.CompletedTask;
    }

    public void Dispose()
    {
        _cts?.Dispose();
    }
}