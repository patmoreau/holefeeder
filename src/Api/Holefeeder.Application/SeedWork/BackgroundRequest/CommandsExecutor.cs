using System.ComponentModel;
using System.Reflection;

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Holefeeder.Application.SeedWork.BackgroundRequest;

public class CommandsExecutor
{
    private readonly IMediator _mediator;
    public CommandsExecutor(IMediator mediator)
    {
        _mediator = mediator;
    }

    [DisplayName("Processing command {0}")]
    public async Task ExecuteCommand(MediatorSerializedObject mediatorSerializedObject)
    {
        var type = Assembly.GetAssembly(typeof(Application))!.GetType(mediatorSerializedObject.FullTypeName);

        if (type != null)
        {
            var req = JsonSerializer.Deserialize(mediatorSerializedObject.Data, type);

            await _mediator.Send((req as IRequest)!);
        }
    }
}
