using Domain.Entities;
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

    Task<List<Record>> GetByUserIdAsync(
        Guid userId,
        int limit,
        int offset,
        bool isPending,
        CancellationToken cancellationToken = default);

    Task<Record?> GetRecordById(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<Record>> GetApprovedRecordsFromTime(DateTime dateTime, CancellationToken cancellationToken = default);
}