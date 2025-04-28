using Application.Abstractions;
using Application.Messaging.Command;
using Domain.Errors;
using Domain.IntegrationEvents.Record;
using Domain.Repositories.Records;
using Domain.Shared;

namespace Application.Features.Records.Commands.DeleteMessage;

public sealed class DeleteMessageCommandHandler(IRecordRepository recordRepository, IIntegrationEventBus eventBus) :
    ICommandHandler<DeleteMessageCommand, Result>
{
    public async Task<Result> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        var record = await recordRepository.GetByIdWithMessages(request.RecordId, cancellationToken);
        if (record is null)
            return Result.Failure(DomainErrors.Venue.NotFound(request.RecordId));

        if (request.InitiatorId != record.StaffId && request.InitiatorId != record.UserId)
            return Result.Failure(DomainErrors.RecordChat.NoAccess(request.InitiatorId));

        if (request.InitiatorId != record.Messages.FirstOrDefault(m => m.Id == request.MessageId)?.SenderId)
            return Result.Failure(DomainErrors.RecordChat.NoAccess(request.InitiatorId));

        var deleteMessageResult = record.DeleteMessage(request.InitiatorId, request.MessageId);
        if (deleteMessageResult.IsFailure)
            return deleteMessageResult;

        await eventBus.SendAsync(
            new RecordDeleteMessageEvent(
                Guid.NewGuid(),
                request.MessageId),
            cancellationToken);

        return Result.Success();
    }
}