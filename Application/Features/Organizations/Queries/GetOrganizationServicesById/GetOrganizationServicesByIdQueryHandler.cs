using Application.Features.Dto;
using Application.Features.Organizations.Queries.GetOrganizationServicesById.Dto;
using Application.Messaging.Query;
using Domain.Repositories.Services;
using Domain.Shared;

namespace Application.Features.Organizations.Queries.GetOrganizationServicesById;

public class GetOrganizationServicesByIdQueryHandler(IServiceReadOnlyRepository serviceReadOnlyRepository)
    : IQueryHandler<GetOrganizationServicesByIdQuery,
        Result<PaginatedVm<ServiceLookupDto>>>
{
    public async Task<Result<PaginatedVm<ServiceLookupDto>>> Handle(
        GetOrganizationServicesByIdQuery request,
        CancellationToken cancellationToken)
    {
        var services = await serviceReadOnlyRepository.GetByOrganizationId(request.Id, cancellationToken);

        if (request.Search is not null)
        {
            services = services.Where(s => s.Name.Value.ToLower().Contains(request.Search.ToLower())).ToList();
            var descriptionServices = services.Where(
                s => (s.Description?.Value.ToLower().Contains(request.Search.ToLower()) ?? false) &&
                     !services.Select(c => c.Id).Contains(s.Id));

            services.AddRange(descriptionServices);
        }

        var serviceDtos = services.Select(
            s => new ServiceLookupDto
            {
                Id = s.Id,
                Name = s.Name.Value,
                Rating = (int)s.Rating.Value,
                Duration = s.Duration,
                Price = s.Price?.Value,
                Photo = s.Photo?.Value,
                StaffIds = s.Staffs.Select(st => st.Id).ToList(),
                VenueIds = s.Venues.Select(v => v.Id).ToList()
            }).ToList();

        // TODO : move to GetByOrganizationId
        var pagedData = serviceDtos
            .Skip((request.Page) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var result = new PaginatedVm<ServiceLookupDto>
        {
            Data = pagedData,
            TotalCount = serviceDtos.Count,
            PageNumber = request.Page,
            PageSize = request.PageSize
        };

        return Result.Success(result);
    }
}