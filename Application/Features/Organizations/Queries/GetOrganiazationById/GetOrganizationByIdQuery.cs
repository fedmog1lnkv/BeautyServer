using Application.Messaging.Query;
using Domain.Entities;
using Domain.Shared;

namespace Application.Features.Organizations.Queries.GetOrganiazationById;

public record GetOrganizationByIdQuery(Guid Id) : IQuery<Result<Organization>>;