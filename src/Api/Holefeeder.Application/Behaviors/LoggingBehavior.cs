using Holefeeder.Application.Extensions;

using Microsoft.Extensions.Logging;

namespace Holefeeder.Application.Behaviors;

internal class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger _logger;

    public LoggingBehavior(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (next == null)
        {
            throw new ArgumentNullException(nameof(next));
        }

        var requestName = typeof(TRequest).FullName ?? nameof(TRequest);

        using var scope = _logger.BeginScope(request);

        _logger.LogMediatrRequest(requestName, request);
        var response = await next();
        _logger.LogMediatrResponse(requestName, response);

        return response;
    }
}
