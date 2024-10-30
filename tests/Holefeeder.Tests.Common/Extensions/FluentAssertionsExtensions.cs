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
        AssertionConfiguration.Current.Equivalency.Modify(options => options.Using(new StronglyTypedIdEquivalencyStep()));
        AssertionConfiguration.Current.Equivalency.Modify(options => options.Using(new MoneyEquivalencyStep()));
        AssertionConfiguration.Current.Equivalency.Modify(options => options.Using(new ColorEquivalencyStep()));
    }
}

internal class MoneyEquivalencyStep : IEquivalencyStep
{
    public EquivalencyResult Handle(Comparands comparands, IEquivalencyValidationContext context,
        IValidateChildNodeEquivalency nestedValidator)
    {
        switch (comparands)
        {
            case {Subject: decimal subjectDecimal, Expectation: Money expectedMoney}:
                subjectDecimal.Should().Be(expectedMoney.Value);
                return EquivalencyResult.EquivalencyProven; // Indicating that the comparison is handled
            case {Subject: Money subjectMoney, Expectation: decimal expectedDecimal}:
                subjectMoney.Value.Should().Be(expectedDecimal);
                return EquivalencyResult.EquivalencyProven; // Indicating that the comparison is handled
            default:
                return EquivalencyResult.ContinueWithNext; // Indicating that the comparison is not handled
        }
    }
}

internal class ColorEquivalencyStep : IEquivalencyStep
{
    public EquivalencyResult Handle(Comparands comparands, IEquivalencyValidationContext context,
        IValidateChildNodeEquivalency nestedValidator)
    {
        switch (comparands)
        {
            case {Subject: string subjectString, Expectation: CategoryColor expectedCategoryColor}:
                subjectString.Should().BeEquivalentTo(expectedCategoryColor.ToString());
                return EquivalencyResult.EquivalencyProven; // Indicating that the comparison is handled
            case {Subject: CategoryColor subjectCategoryColor, Expectation: string expectedString}:
                subjectCategoryColor.ToString().Should().BeEquivalentTo(expectedString);
                return EquivalencyResult.EquivalencyProven; // Indicating that the comparison is handled
            default:
                return EquivalencyResult.ContinueWithNext; // Indicating that the comparison is not handled
        }
    }
}
