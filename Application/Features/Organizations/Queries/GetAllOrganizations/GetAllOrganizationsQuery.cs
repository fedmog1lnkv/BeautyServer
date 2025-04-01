using Application.Messaging.Query;
using Domain.Entities;
using Domain.Shared;

namespace Application.Features.Organizations.Queries.GetAllOrganizations;

public record GetAllOrganizationsQuery(
    int Limit,
    int Offset) : IQuery<Result<List<Organization>>>;