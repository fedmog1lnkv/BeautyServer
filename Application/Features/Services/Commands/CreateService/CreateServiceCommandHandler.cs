using Application.Messaging.Command;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Organizations;
using Domain.Repositories.Services;
using Domain.Shared;

namespace Application.Features.Services.Commands.CreateService;

internal sealed class CreateServiceCommandHandler(
    IServiceRepository serviceRepository,
    IOrganizationReadOnlyRepository organizationReadOnlyRepository)
    : ICommandHandler<CreateServiceCommand, Result>
{
    public async Task<Result> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var organizationExists =
            await organizationReadOnlyRepository.ExistsAsync(request.OrganizationId, cancellationToken);
        if (!organizationExists)
            return Result.Failure(DomainErrors.Organization.NotFound(request.OrganizationId));

        var createServiceResult = await Service.Create(
            Guid.NewGuid(),
            request.OrganizationId,
            request.Name,
            DateTime.UtcNow,
            organizationReadOnlyRepository);

        if (createServiceResult.IsFailure)
            return Result.Failure(createServiceResult.Error);

        var service = createServiceResult.Value;

        if (!string.IsNullOrEmpty(request.Description))
        {
            var descriptionResult = service.SetDescription(request.Description);
            if (descriptionResult.IsFailure)
                return Result.Failure(descriptionResult.Error);
        }

        if (request.Duration.HasValue)
        {
            var durationResult = service.SetDuration(request.Duration.Value);
            if (durationResult.IsFailure)
                return Result.Failure(durationResult.Error);
        }

        if (request.Price.HasValue)
        {
            var priceResult = service.SetPrice(request.Price.Value);
            if (priceResult.IsFailure)
                return Result.Failure(priceResult.Error);
        }

        serviceRepository.Add(service);

        return Result.Success();
    }
}