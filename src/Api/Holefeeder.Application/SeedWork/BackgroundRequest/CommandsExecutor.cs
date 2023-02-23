using System.ComponentModel;
using System.Reflection;
using System.Text.Json;

namespace Holefeeder.Application.SeedWork.BackgroundRequest;

internal class CommandsExecutor
{
    private readonly IMediator _mediator;

    public CommandsExecutor(IMediator mediator) => _mediator = mediator;

    [DisplayName("Processing command {0}")]
    public async Task ExecuteCommand(MediatorSerializedObject mediatorSerializedObject)
    {
        Type? type = Assembly.GetAssembly(typeof(Application))!.GetType(mediatorSerializedObject.FullTypeName);

        if (type != null)
        {
            object? req = JsonSerializer.Deserialize(mediatorSerializedObject.Data, type);

            await _mediator.Send((req as IRequest<Unit>)!);
        }
    }
}
