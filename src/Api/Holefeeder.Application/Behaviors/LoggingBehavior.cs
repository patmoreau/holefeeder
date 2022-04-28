using MediatR;

using Microsoft.Extensions.Logging;

namespace Holefeeder.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger;

    // ReSharper disable once ContextualLoggerProblem
    public LoggingBehavior(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        var requestName = typeof(TRequest).Name;

        using var scope = _logger.BeginScope(request);

        _logger.LogInformation("API Request: {Name} {@Request}", requestName, request);
        var response = await next();
        _logger.LogInformation("API Request: {Name} with result {Response}", requestName, response);
        return response;
    }
}
