using Application.Abstractions;
using Domain.DomainEvents.Organizations;
using Domain.DomainEvents.Record;
using Domain.Repositories;
using Domain.Repositories.Records;
using Domain.Repositories.Staffs;
using Serilog;

namespace Application.Features.Staffs.Events;

public class RecordReviewAddedChangedEventHandler(
    IStaffRepository staffRepository,
    IRecordRepository recordRepository)
    : IDomainEventHandler<RecordReviewAddedChangedEvent>
{
    public async Task Handle(RecordReviewAddedChangedEvent notification, CancellationToken cancellationToken)
    {
        var staff = await staffRepository.GetByIdAsync(notification.StaffId, cancellationToken);
        if (staff is null)
        {
            Log.Error(
                $"Notification failure {nameof(OrganizationColorChangedEvent)} for staff {notification.StaffId}. Staff not found.");
            return;
        }

        var records = await recordRepository.GetByStaffId(
            staff.Id,
            int.MaxValue,
            0,
            cancellationToken);

        var recordsWithReviews = records.Where(r => r.Review != null).ToList();
        var avgRating = 0.0;

        if (recordsWithReviews.Any())
            avgRating = recordsWithReviews.Average(r => r.Review?.Rating ?? 0);

        avgRating = Math.Round((avgRating + notification.Rating) / 2, 2);

        var result = staff.SetRating(avgRating);
        if (result.IsFailure)
            Log.Error($"Failed to set rating for staff {staff.Id}. Error: {result.Error}");
    }
}