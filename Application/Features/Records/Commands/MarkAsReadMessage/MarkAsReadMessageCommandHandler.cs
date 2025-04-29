using Application.Abstractions;
using Application.Messaging.Command;
using Domain.Errors;
using Domain.IntegrationEvents.Record;
using Domain.Repositories.Records;
using Domain.Shared;

namespace Application.Features.Records.Commands.MarkAsReadMessage;

public sealed class MarkAsReadMessageCommandHandler(IRecordRepository recordRepository, IIntegrationEventBus eventBus) :
    ICommandHandler<MarkAsReadMessageCommand, Result>
{
    public async Task<Result> Handle(MarkAsReadMessageCommand request, CancellationToken cancellationToken)
    {
        var record = await recordRepository.GetByIdWithMessages(request.RecordId, cancellationToken);
        if (record is null)
            return Result.Failure(DomainErrors.Venue.NotFound(request.RecordId));

        if (request.ReaderId != record.StaffId && request.ReaderId != record.UserId)
            return Result.Failure(DomainErrors.RecordChat.NoAccess(request.ReaderId));

        foreach (var messageId in request.MessageIds)
        {
            if (record.Messages.FirstOrDefault(m => m.Id == messageId) is { IsRead: true })
                continue;
            
            var readMessageResult = record.MarkMessageAsRead(messageId, request.ReaderId);
            if (readMessageResult.IsFailure)
                return readMessageResult;

            await eventBus.SendAsync(
                new RecordReadMessageEvent(
                    Guid.NewGuid(),
                    record.Id,
                    request.ReaderId,
                    messageId),
                cancellationToken);
        }


        return Result.Success();
    }
}