using Application.Abstractions;
using Application.Messaging.Command;
using Domain.Repositories;
using Domain.Shared;
using MediatR;
using Serilog;

namespace Application.Common.Behaviors;

public class UnitOfWorkPipelineBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork, IDomainEventBus eventBus)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next();
        if (response.IsSuccess)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await PublishDomainEvents(cancellationToken);
        }

        return response;
    }

    private async Task PublishDomainEvents(CancellationToken cancellationToken)
    {
        var domainEvents = unitOfWork.GetDomainEvents();

        if (domainEvents.Any())
        {
            try
            {
                foreach (var domainEvent in domainEvents)
                {
                    await eventBus.SendAsync(domainEvent, cancellationToken);
                }

                unitOfWork.ClearDomainEvents();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error publishing domain events");
            }
        }
    }
}