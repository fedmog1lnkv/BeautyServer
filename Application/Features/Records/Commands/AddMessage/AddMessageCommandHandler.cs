using Application.Abstractions;
using Application.Messaging.Command;
using Domain.Errors;
using Domain.IntegrationEvents.Record;
using Domain.Repositories.Records;
using Domain.Shared;

namespace Application.Features.Records.Commands.AddMessage;

public sealed class AddMessageCommandHandler(IRecordRepository recordRepository, IIntegrationEventBus eventBus) :
    ICommandHandler<AddMessageCommand, Result>
{
    public async Task<Result> Handle(AddMessageCommand request, CancellationToken cancellationToken)
    {
        var record = await recordRepository.GetByIdWithMessages(request.RecordId, cancellationToken);
        if (record is null)
            return Result.Failure(DomainErrors.Venue.NotFound(request.RecordId));

        if (request.SenderId != record.StaffId && request.SenderId != record.UserId)
            return Result.Failure(DomainErrors.RecordChat.NoAccess(request.SenderId));

        var messageText = request.Text;

        var isBase64 = Utils.IsBase64String(request.Text);
        if (isBase64)
        {
            var photoUrl = await recordRepository.UploadMessagePhotoAsync(request.Text, Guid.NewGuid().ToString());

            if (string.IsNullOrEmpty(photoUrl))
                return Result.Failure(DomainErrors.RecordMessage.PhotoUploadFailed);

            messageText = photoUrl;
        }

        var addMessageResult = record.AddMessage(request.SenderId, messageText, request.Id);
        if (addMessageResult.IsFailure)
            return addMessageResult;

        await eventBus.SendAsync(
            new RecordAddMessageEvent(
                Guid.NewGuid(),
                record.Id,
                request.SenderId,
                record.Messages.Last().Id,
                record.Messages.Last().Message.Value,
                record.Messages.Last().CreatedAt),
            cancellationToken);

        return Result.Success();
    }
}