using Domain.Entities;
using Domain.Enums;
using Domain.Repositories.Base;

namespace Domain.Repositories.Records;

public interface IRecordReadOnlyRepository : IReadOnlyRepository<Record>
{
    Task<List<Record>> GetByStaffIdAndDateAsync(
        Guid staffId,
        DateOnly date,
        int limit,
        int offset,
        bool isPending,
        CancellationToken cancellationToken = default);

    Task<List<Record>> GetByStaffIdAndMonth(
        Guid staffId,
        int year,
        int month,
        CancellationToken cancellationToken = default);

    Task<List<Record>> GetRecordsWithVenueByStaffIdAndDate(
        Guid staffId,
        DateOnly date,
        CancellationToken cancellationToken = default);

    Task<List<Record>> GetByUserId(
        Guid userId,
        int limit,
        int offset,
        CancellationToken cancellationToken = default);
    
    Task<List<Record>> GetRecordsByEntityId(
        EntityType entityType,
        Guid entityId,
        int limit,
        int offset,
        CancellationToken cancellationToken = default);

    Task<int> GetTotalCountByEntityId(
        EntityType entityType,
        Guid entityId,
        CancellationToken cancellationToken = default);
    
    Task<int> GetTotalCountByEntityIdWithReviews(
        EntityType entityType,
        Guid entityId,
        CancellationToken cancellationToken = default);

    Task<List<Record>> GetRecordsByEntityIdWithReviews(
        EntityType entityType,
        Guid entityId,
        int limit,
        int offset,
        CancellationToken cancellationToken = default);


    Task<Record?> GetRecordById(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Record?> GetByIdWithMessagesAndStatusLog(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<Record>> GetByOrganizationIdAsync(Guid organizationId, CancellationToken cancellationToken = default);

    Task<List<Record>> GetApprovedRecordsFromTime(DateTime dateTime, CancellationToken cancellationToken = default);
}