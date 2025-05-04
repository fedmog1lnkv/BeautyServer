using Application.Messaging.Query;
using Domain.Entities;
using Domain.Shared;

namespace Application.Features.Services.Queries.GetServiceById;

public record GetServiceByIdQuery(Guid Id) : IQuery<Result<Service>>;