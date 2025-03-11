using Domain.Primitives;

namespace Domain.Repositories.Base;

public interface IRepository<T> where T : AggregateRoot { }