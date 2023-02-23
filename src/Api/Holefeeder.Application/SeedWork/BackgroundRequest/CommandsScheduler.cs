using System.Text.Json;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace Holefeeder.Application.SeedWork.BackgroundRequest;

internal class CommandsScheduler
{
    private readonly CommandsExecutor _commandsExecutor;
    private readonly IServiceProvider _serviceProvider;

    public CommandsScheduler(CommandsExecutor commandsExecutor, IServiceProvider serviceProvider)
    {
        _commandsExecutor = commandsExecutor;
        _serviceProvider = serviceProvider;
    }

    public string SendNow(IRequest<Unit> request, string description)
    {
        MediatorSerializedObject mediatorSerializedObject = SerializeObject(request, description);

        IBackgroundJobClient? job = _serviceProvider.GetService<IBackgroundJobClient>();
        return job.Enqueue(() => _commandsExecutor.ExecuteCommand(mediatorSerializedObject));
    }

    public string SendNow(IRequest request, string parentJobId, JobContinuationOptions continuationOption,
        string description)
    {
        MediatorSerializedObject mediatorSerializedObject = SerializeObject(request, description);
        IBackgroundJobClient? job = _serviceProvider.GetService<IBackgroundJobClient>();
#pragma warning disable CS4014
        return job.ContinueJobWith(parentJobId,
            () => _commandsExecutor.ExecuteCommand(mediatorSerializedObject), continuationOption);
#pragma warning restore CS4014
    }

    public void Schedule(IRequest request, DateTimeOffset scheduleAt, string description)
    {
        MediatorSerializedObject mediatorSerializedObject = SerializeObject(request, description);

        IBackgroundJobClient? job = _serviceProvider.GetService<IBackgroundJobClient>();
        job.Schedule(() => _commandsExecutor.ExecuteCommand(mediatorSerializedObject), scheduleAt);
    }

    public void Schedule(IRequest request, TimeSpan delay, string description)
    {
        MediatorSerializedObject mediatorSerializedObject = SerializeObject(request, description);
        DateTime newTime = DateTime.Now + delay;
        IBackgroundJobClient? job = _serviceProvider.GetService<IBackgroundJobClient>();
        job.Schedule(() => _commandsExecutor.ExecuteCommand(mediatorSerializedObject), newTime);
    }

    public void ScheduleRecurring(IRequest request, string name, string cronExpression, string description)
    {
        MediatorSerializedObject mediatorSerializedObject = SerializeObject(request, description);

        IRecurringJobManager? job = _serviceProvider.GetService<IRecurringJobManager>();
        job.AddOrUpdate(name, () => _commandsExecutor.ExecuteCommand(mediatorSerializedObject), cronExpression,
            TimeZoneInfo.Local);
    }

    private static MediatorSerializedObject SerializeObject(object mediatorObject, string description)
    {
        string fullTypeName = mediatorObject.GetType().FullName ?? mediatorObject.GetType().Name;
        string data = JsonSerializer.Serialize(mediatorObject);

        return new MediatorSerializedObject(fullTypeName, data, description);
    }
}
