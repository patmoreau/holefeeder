using FluentAssertions;
using Holefeeder.Tests.Common.SeedWork.Drivers;

namespace Holefeeder.Tests.Common.SeedWork.StepDefinitions;

public abstract class RootStepDefinition
{
    protected HttpClientDriver HttpClientDriver { get; }

    protected RootStepDefinition(HttpClientDriver httpClientDriver) => HttpClientDriver = httpClientDriver;

    public Guid WithCreatedId()
    {
        Created? result = HttpClientDriver.DeserializeContent<Created>();

        result.Should().NotBeNull();

        return result!.Id;
    }

    public TResult WithResultAs<TResult>()
    {
        TResult? result = HttpClientDriver.DeserializeContent<TResult>();
        result.Should().NotBeNull();

        return result!;
    }

    private record Created(Guid Id);
}
