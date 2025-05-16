using Application.Features.Dto;
using Application.Features.Organizations.Queries.GetOrganizationStaffsById.Dto;
using Application.Messaging.Query;
using Domain.Repositories.Staffs;
using Domain.Shared;

namespace Application.Features.Organizations.Queries.GetOrganizationStaffsById;

public class GetOrganizationStaffsByIdQueryHandler(IStaffReadOnlyRepository staffReadOnlyRepository)
    : IQueryHandler<GetOrganizationStaffsByIdQuery,
        Result<PaginatedVm<StaffLookupDto>>>
{
    public async Task<Result<PaginatedVm<StaffLookupDto>>> Handle(
        GetOrganizationStaffsByIdQuery request,
        CancellationToken cancellationToken)
    {
        var staffs = await staffReadOnlyRepository.GetByOrganizationId(request.Id, cancellationToken);

        if (request.Search is not null)
        {
            staffs = staffs.Where(s => s.Name.Value.ToLower().Contains(request.Search.ToLower())).ToList();
        }

        var staffDtos = staffs.Select(
            s => new StaffLookupDto
            {
                Id = s.Id,
                Name = s.Name.Value,
                Rating = (int)s.Rating.Value,
                Role = s.Role.ToString(),
                PhoneNumber = s.PhoneNumber.Value,
                Photo = s.Photo?.Value
            }).ToList();

        // TODO : move to GetByOrganizationId
        var pagedData = staffDtos
            .Skip((request.Page) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var result = new PaginatedVm<StaffLookupDto>
        {
            Data = pagedData,
            TotalCount = staffDtos.Count,
            PageNumber = request.Page,
            PageSize = request.PageSize
        };

        return Result.Success(result);
    }
}