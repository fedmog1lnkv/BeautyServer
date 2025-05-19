using Application.Messaging.Command;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using Domain.Repositories.Coupons;
using Domain.Repositories.Organizations;
using Domain.Repositories.Records;
using Domain.Repositories.Services;
using Domain.Repositories.Staffs;
using Domain.Repositories.Users;
using Domain.Repositories.Utils;
using Domain.Repositories.Venues;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Features.Records.Commands.CreateRecord;

public class CreateRecordCommandHandler(
    IUserReadOnlyRepository userReadOnlyRepository,
    IStaffRepository staffRepository,
    IStaffReadOnlyRepository staffReadOnlyRepository,
    IOrganizationReadOnlyRepository organizationReadOnlyRepository,
    IVenueReadOnlyRepository venueReadOnlyRepository,
    IServiceReadOnlyRepository serviceReadOnlyRepository,
    IRecordRepository recordRepository,
    ICouponRepository couponRepository,
    INotificationRepository notificationRepository) : ICommandHandler<CreateRecordCommand, Result>
{
    public async Task<Result> Handle(CreateRecordCommand request, CancellationToken cancellationToken)
    {
        var staff = await staffRepository.GetByIdAsync(request.StaffId, cancellationToken);
        if (staff is null)
            return Result.Failure(DomainErrors.Staff.NotFound(request.StaffId));

        var service = staff.Services.FirstOrDefault(s => s.Id == request.ServiceId);
        if (service is null)
            return Result.Failure(DomainErrors.Service.NotFound(request.ServiceId));

        var endTimeStamp = request.EndTimeStamp ??
                           request.StartTimestamp + (service.Duration ?? TimeSpan.FromMinutes(30));

        var timeSlot = staff.TimeSlots.FirstOrDefault(ts => ts.Date == DateOnly.FromDateTime(request.StartTimestamp));
        if (timeSlot is null)
            return Result.Failure(DomainErrors.TimeSlot.NotFoundByTime);

        var startTime = request.StartTimestamp.TimeOfDay;
        var endTime = endTimeStamp.TimeOfDay;

        var isAvailableTime = timeSlot.Intervals.Any(
            interval =>
                startTime < interval.End && endTime > interval.Start);

        if (!isAvailableTime)
            return Result.Failure(DomainErrors.TimeSlot.IntervalsOverlap);

        var updatedIntervals = new List<Interval>();

        foreach (var interval in timeSlot.Intervals)
        {
            if (startTime <= interval.End && endTime >= interval.Start)
            {
                if (interval.Start != startTime)
                {
                    var createIntervalResult = Interval.Create(interval.Start, startTime);
                    if (createIntervalResult.IsFailure)
                        return createIntervalResult;

                    updatedIntervals.Add(createIntervalResult.Value);
                }

                if (interval.End != endTime)
                {
                    var createIntervalResult = Interval.Create(endTime, interval.End);
                    if (createIntervalResult.IsFailure)
                        return createIntervalResult;

                    updatedIntervals.Add(createIntervalResult.Value);
                }
            }
            else
            {
                updatedIntervals.Add(interval);
            }
        }

        var updateStaffIntervalsResult = staff.UpdateTimeSlotIntervals(timeSlot.Id, updatedIntervals);
        if (updateStaffIntervalsResult.IsFailure)
            return Result.Failure(updateStaffIntervalsResult.Error);

        var venue = await venueReadOnlyRepository.GetByIdAsync(timeSlot.VenueId, cancellationToken);
        if (venue is null)
            return Result.Failure(DomainErrors.Venue.NotFound(timeSlot.VenueId));

        var price = (decimal?)service.Price?.Value;
        var discountPrice = price;

        Coupon? coupon = null;

        var hasCoupon = request.CouponId.HasValue && request.CouponId != Guid.Empty;

        if (price.HasValue && hasCoupon)
        {
            coupon = await couponRepository.GetByIdAsync(request.CouponId!.Value, cancellationToken);

            if (coupon is null)
                return Result.Failure(DomainErrors.Coupon.NotFound(request.CouponId.Value));

            if (coupon.UsageLimit.Remaining == 0)
                return Result.Failure(DomainErrors.Coupon.NoRemainingUses);

            discountPrice = coupon.ApplyDiscount(price.Value);
        }

        var createRecordResult = await Record.CreateAsync(
            Guid.NewGuid(),
            request.UserId,
            request.StaffId,
            staff.OrganizationId,
            timeSlot.VenueId,
            service.Id,
            request.CouponId,
            price,
            discountPrice,
            RecordStatus.Pending,
            TimeZoneInfo.ConvertTimeToUtc(request.StartTimestamp, venue.TimeZone),
            TimeZoneInfo.ConvertTimeToUtc(endTimeStamp, venue.TimeZone),
            DateTime.UtcNow,
            userReadOnlyRepository,
            staffReadOnlyRepository,
            organizationReadOnlyRepository,
            venueReadOnlyRepository,
            serviceReadOnlyRepository,
            cancellationToken);

        if (createRecordResult.IsFailure)
            return createRecordResult;

        recordRepository.Add(createRecordResult.Value);

        if (coupon is not null)
        {
            var useResult = coupon.UseOnce();
            if (useResult.IsFailure)
                return useResult;
        }

        var record = createRecordResult.Value;
        if (record.Staff.Settings.FirebaseToken != null)
        {
            var recordStartLocalTime = TimeZoneInfo.ConvertTimeFromUtc(record.StartTimestamp.DateTime, venue.TimeZone);
            await notificationRepository.SendOrderNotificationAsync(
                record.Id,
                record.Staff.Settings.FirebaseToken,
                "Новая запись",
                $"У вас новая запись на услугу «{record.Service.Name.Value}» от клиента {record.User.Name} на {recordStartLocalTime:dd.MM.yyyy в HH:mm}.");
        }

        return Result.Success();
    }
}