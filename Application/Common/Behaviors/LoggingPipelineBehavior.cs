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
        var json = JsonSerializer.Serialize(
            request,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });
        Log.Information(
            "Starting request {@RequestName}\n{@RequestBody}",
            typeof(TRequest).Name,
            json);

        var result = await next();

        if (result.IsFailure)
            Log.Error(
                "Request failure {RequestName}\nError: {Error}",
                typeof(TRequest).Name,
                result.Error);

        Log.Information(
            "Completed request {RequestName}",
            typeof(TRequest).Name);

        return result;
    }
}