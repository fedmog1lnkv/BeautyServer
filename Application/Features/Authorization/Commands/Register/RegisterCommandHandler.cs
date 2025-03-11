using Application.Abstractions;
using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Authorization.Commands.Register;

internal sealed class RegisterCommandHandler(IJwtProvider jwtProvider)
    : ICommandHandler<RegisterCommand, Result<string>>
{
    public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return "123";
    }
}