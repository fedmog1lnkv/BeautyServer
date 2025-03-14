using Domain.Errors;
using Domain.Primitives;
using Domain.Repositories.Organizations;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Service : AggregateRoot
{
    private Service(
        Guid id,
        Guid organizationId,
        ServiceName name,
        ServiceDescription? description,
        TimeSpan? duration,
        ServicePrice? price) : base(id)
    {
        OrganizationId = organizationId;
        Name = name;
        Description = description;
        Duration = duration;
        Price = price;
    }

#pragma warning disable CS8618
    private Service() { }
#pragma warning restore CS8618

    public Guid OrganizationId { get; private set; }
    public ServiceName Name { get; private set; }
    public ServiceDescription? Description { get; private set; }
    public TimeSpan? Duration { get; private set; }
    public ServicePrice? Price { get; private set; }

    public static async Task<Result<Service>> Create(
        Guid id,
        Guid organizationId,
        string name,
        IOrganizationReadOnlyRepository repository)
    {
        if (organizationId == Guid.Empty)
            return Result.Failure<Service>(DomainErrors.Service.OrganizationIdEmpty);

        var organizationExists = await repository.ExistsAsync(organizationId);
        if (!organizationExists)
            return Result.Failure<Service>(DomainErrors.Organization.NotFound(organizationId));

        var nameResult = ServiceName.Create(name);
        if (nameResult.IsFailure)
            return Result.Failure<Service>(nameResult.Error);

        return new Service(
            id,
            organizationId,
            nameResult.Value,
            null,
            null,
            null);
    }

    public Result SetName(string name)
    {
        var nameResult = ServiceName.Create(name);
        if (nameResult.IsFailure)
            return nameResult;

        if (Name.Equals(nameResult.Value))
            return Result.Success();

        Name = nameResult.Value;
        return Result.Success();
    }

    public Result SetDescription(string description)
    {
        var descriptionResult = ServiceDescription.Create(description);
        if (descriptionResult.IsFailure)
            return descriptionResult;

        if (Description is not null && Description.Equals(descriptionResult.Value))
            return Result.Success();

        Description = descriptionResult.Value;
        return Result.Success();
    }

    public Result SetDuration(int durationMinutes)
    {
        if (durationMinutes <= 0)
            return Result.Failure(DomainErrors.Service.InvalidDuration);

        var newDuration = TimeSpan.FromMinutes(durationMinutes);
        if (Duration.HasValue && Duration.Value == newDuration)
            return Result.Success();

        Duration = newDuration;
        return Result.Success();
    }

    public Result SetPrice(double price)
    {
        var priceResult = ServicePrice.Create(price);
        if (priceResult.IsFailure)
            return priceResult;

        if (Price is not null && Price.Equals(priceResult.Value))
            return Result.Success();

        Price = priceResult.Value;
        return Result.Success();
    }
}
