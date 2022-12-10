namespace Holefeeder.Application.SeedWork;

internal interface ICommandRequest<out TResponse> : IRequest<TResponse>
{
}
