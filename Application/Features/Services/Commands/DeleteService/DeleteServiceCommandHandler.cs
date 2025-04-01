using Application.Abstractions;
using Application.Features.Organizations.Commands.DeleteOrganization;
using Application.Messaging.Command;
using Domain.Errors;
using Domain.Repositories.Organizations;
using Domain.Repositories.Services;
using Domain.Shared;

namespace Application.Features.Services.Commands.DeleteService;

public sealed class DeleteServiceCommandHandler(IServiceRepository serviceRepository)
    : ICommandHandler<DeleteServiceCommand, Result>
{
    public async Task<Result> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await serviceRepository.GetById(request.Id, cancellationToken);

        if (service is null)
            return Result.Failure(DomainErrors.Service.NotFound(request.Id));
        
        serviceRepository.Remove(service);

        return Result.Success();
    }
}