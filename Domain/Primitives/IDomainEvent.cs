using MediatR;

namespace Domain.Primitives;

public interface IDomainEvent
{
    public Guid Id { get; init; }
}