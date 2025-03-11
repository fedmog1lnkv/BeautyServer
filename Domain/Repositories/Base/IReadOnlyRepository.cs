using Domain.Primitives;

namespace Domain.Repositories.Base;

public interface IReadOnlyRepository<T> where T : AggregateRoot { }