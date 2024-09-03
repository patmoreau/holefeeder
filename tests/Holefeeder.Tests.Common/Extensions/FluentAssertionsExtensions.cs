using DrifterApps.Seeds.Domain;

using FluentAssertions;
using FluentAssertions.Equivalency;

using Holefeeder.Domain.ValueObjects;

// ReSharper disable once CheckNamespace
namespace FluentAssertionsEquivalency;

public static class FluentAssertionsExtensions
{
    public static void RegisterGlobalEquivalencySteps()
    {
        AssertionOptions.AssertEquivalencyUsing(options => options.Using(new StronglyTypedIdEquivalencyStep()));
        AssertionOptions.AssertEquivalencyUsing(options => options.Using(new MoneyEquivalencyStep()));
        AssertionOptions.AssertEquivalencyUsing(options => options.Using(new ColorEquivalencyStep()));
    }
}

internal class StronglyTypedIdEquivalencyStep : IEquivalencyStep
{
    public EquivalencyResult Handle(Comparands comparands, IEquivalencyValidationContext context,
        IEquivalencyValidator nestedValidator)
    {
        if (comparands is not { Subject: Guid subject, Expectation: IStronglyTypedId expected })
        {
            return EquivalencyResult.ContinueWithNext; // Indicating that the comparison is not handled
        }

        subject.Should().Be(expected.Value);

        return EquivalencyResult.AssertionCompleted; // Indicating that the comparison is handled
    }
}

internal class MoneyEquivalencyStep : IEquivalencyStep
{
    public EquivalencyResult Handle(Comparands comparands, IEquivalencyValidationContext context,
        IEquivalencyValidator nestedValidator)
    {
        if (comparands is not { Subject: decimal subject, Expectation: Money expected })
        {
            return EquivalencyResult.ContinueWithNext; // Indicating that the comparison is not handled
        }

        subject.Should().Be(expected.Value);

        return EquivalencyResult.AssertionCompleted; // Indicating that the comparison is handled
    }
}

internal class ColorEquivalencyStep : IEquivalencyStep
{
    public EquivalencyResult Handle(Comparands comparands, IEquivalencyValidationContext context,
        IEquivalencyValidator nestedValidator)
    {
        if (comparands is not { Subject: string subject, Expectation: CategoryColor expected })
        {
            return EquivalencyResult.ContinueWithNext; // Indicating that the comparison is not handled
        }

        subject.Should().Be(expected.ToString());

        return EquivalencyResult.AssertionCompleted; // Indicating that the comparison is handled
    }
}
