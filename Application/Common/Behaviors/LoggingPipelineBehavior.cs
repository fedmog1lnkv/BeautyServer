using Domain.Shared;
using MediatR;
using Serilog;
using System.Text.Json;

namespace Application.Common.Behaviors;

public class LoggingPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        Log.Information("Starting request {@RequestName}, {@DateTimeUtc}, {@RequestBody}",
            typeof(TRequest).Name,
            DateTime.UtcNow,
            JsonSerializer.Serialize(request));

        var result = await next();

        if (result.IsFailure)
            Log.Error("Request failure {@RequestName}, {@Error},{@DateTimeUtc}",
                typeof(TRequest).Name,
                result.Error,
                DateTime.UtcNow);

        Log.Information("Completed request {@RequestName}, {@DateTimeUtc}",
            typeof(TRequest).Name,
            DateTime.UtcNow);

        return result;
    }
}