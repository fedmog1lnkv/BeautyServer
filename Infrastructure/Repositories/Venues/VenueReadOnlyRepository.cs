using Domain.Entities;
using Domain.Repositories.Venues;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Venues;

public class VenueReadOnlyRepository(ApplicationDbContext dbContext) : IVenueReadOnlyRepository
{
    public async Task<bool> ExistsAsync(Guid venueId, CancellationToken cancellationToken = default) =>
        await dbContext.Set<Venue>().AsNoTracking().AnyAsync(v => v.Id == venueId, cancellationToken);

    public async Task<List<Venue>> GetByLocation(
        double latitude,
        double longitude,
        int limit,
        int offset,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Set<Venue>()
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(v => v.Name.Value.ToLower().Contains(search.ToLower()));

        var venues = await query
            .ToListAsync(cancellationToken);

        var result = venues
            .Select(
                v => new
                {
                    Venue = v,
                    Distance = 6371 * Math.Acos(
                        Math.Cos(DegToRad(latitude)) * Math.Cos(DegToRad(v.Location.Latitude)) *
                        Math.Cos(DegToRad(v.Location.Longitude) - DegToRad(longitude)) +
                        Math.Sin(DegToRad(latitude)) * Math.Sin(DegToRad(v.Location.Latitude)))
                })
            .OrderBy(v => v.Distance)
            .Skip(offset)
            .Take(limit)
            .Select(v => v.Venue)
            .ToList();

        return result;
    }

    public async Task<List<Venue>> GetAll(
        int limit,
        int offset,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Set<Venue>()
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(v => v.Name.Value.ToLower().Contains(search.ToLower()));

        return await query
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Venue>> GetInBounds(
        double minLatitude,
        double minLongitude,
        double maxLatitude,
        double maxLongitude,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Set<Venue>()
            .AsNoTracking()
            .Where(
                v =>
                    v.Location.Latitude >= minLatitude &&
                    v.Location.Latitude <= maxLatitude &&
                    v.Location.Longitude >= minLongitude &&
                    v.Location.Longitude <= maxLongitude);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(v => v.Name.Value.ToLower().Contains(search.ToLower()));

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Venue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Set<Venue>()
            .AsNoTracking()
            .Include(v => v.Photos)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

    public async Task<Venue?> GetByIdWithServicesAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Set<Venue>()
            .AsNoTracking()
            .Include(v => v.Services)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

    private static double DegToRad(double deg) => deg * (Math.PI / 180);
}