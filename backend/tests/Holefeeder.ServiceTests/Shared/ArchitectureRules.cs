using System.Text.RegularExpressions;

using ArchUnitNET.Loader;
using ArchUnitNET.xUnitV3;

using Holefeeder.Service;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Holefeeder.ServiceTests.Shared;

public class ArchitectureRules
{
    private const string Root = @"^Holefeeder\.Service\.";
    private const string RingOwner = @"(Features\.[^.]+|Shared)\.";
    private const string NamespaceEnd = @"(\.|$)";

    private static readonly string[] OuterRings = ["Api", "Events", "Jobs", "Persistence", "Ui"];

    private static readonly ArchUnitNET.Domain.Architecture Service =
        new ArchLoader().LoadAssemblies(typeof(Holefeeder.Service.Api).Assembly).Build();

    public static TheoryData<string> LegacyNamespaces =>
    [
        "Holefeeder.Domain",
        "Holefeeder.Application",
        "Holefeeder.Infrastructure",
        "DrifterApps.Seeds.Application",
        "DrifterApps.Seeds.Domain",
        "DrifterApps.Seeds.Infrastructure",
        "DrifterApps.Seeds.Testing"
    ];

    public static TheoryData<string> Rings => [.. OuterRings];

    [Theory]
    [MemberData(nameof(LegacyNamespaces))]
    public void NewCodeDoesNotUseLegacyNamespaces(string legacyNamespace) =>
        Types().That().ResideInNamespaceMatching($"{Root}(Features|Shared){NamespaceEnd}")
            .Should().NotDependOnAnyTypesThat()
            .ResideInNamespaceMatching($"^{Regex.Escape(legacyNamespace)}{NamespaceEnd}")
            .WithoutRequiringPositiveResults()
            .Check(Service);

    [Fact]
    public void CoreDoesNotDependOnAnyRing() =>
        Types().That().ResideInNamespaceMatching($"{Root}{RingOwner}Core{NamespaceEnd}")
            .Should().NotDependOnAnyTypesThat()
            .ResideInNamespaceMatching($"{Root}{RingOwner}({string.Join('|', OuterRings)}){NamespaceEnd}")
            .WithoutRequiringPositiveResults()
            .Check(Service);

    [Theory]
    [MemberData(nameof(Rings))]
    public void RingDoesNotDependOnAnotherRing(string ring) =>
        Types().That().ResideInNamespaceMatching($"{Root}{RingOwner}{ring}{NamespaceEnd}")
            .Should().NotDependOnAnyTypesThat()
            .ResideInNamespaceMatching(
                $"{Root}{RingOwner}({string.Join('|', OuterRings.Where(other => other != ring))}){NamespaceEnd}")
            .WithoutRequiringPositiveResults()
            .Check(Service);

    [Fact]
    public void SharedDoesNotDependOnFeatures() =>
        Types().That().ResideInNamespaceMatching($"{Root}Shared{NamespaceEnd}")
            .Should().NotDependOnAnyTypesThat()
            .ResideInNamespaceMatching($"{Root}Features{NamespaceEnd}")
            .WithoutRequiringPositiveResults()
            .Check(Service);

    [Fact]
    public void FeatureDoesNotDependOnAnotherFeature()
    {
        var features = Service.Types
            .Select(type => Regex.Match(type.Namespace.FullName, $@"{Root}Features\.([^.]+)"))
            .Where(match => match.Success)
            .Select(match => match.Groups[1].Value)
            .Distinct();

        foreach (var feature in features)
        {
            Types().That().ResideInNamespaceMatching($@"{Root}Features\.{feature}{NamespaceEnd}")
                .Should().NotDependOnAnyTypesThat()
                .ResideInNamespaceMatching($@"{Root}Features\.(?!{feature}{NamespaceEnd})")
                .Check(Service);
        }
    }
}
