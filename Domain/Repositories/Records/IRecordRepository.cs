using Domain.Entities;
using Domain.Repositories.Base;

namespace Domain.Repositories.Records;

public interface IRecordRepository : IRepository<Record>
{
    Task<Record?> GetRecordById(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Record?> GetByIdWithMessages(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<Record>> GetByStaffId(
        Guid staffId,
        int limit,
        int offset,
        CancellationToken cancellationToken = default);

    Task<List<Record>> GetByServiceId(
        Guid serviceId,
        int limit,
        int offset,
        CancellationToken cancellationToken = default);

    Task<List<Record>> GetByVenueId(
        Guid venueId,
        int limit,
        int offset,
        CancellationToken cancellationToken = default);
    
    Task<string?> UploadMessagePhotoAsync(string base64Photo, string fileName);

    void Add(Record record);
}