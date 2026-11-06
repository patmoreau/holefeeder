using System.Text.RegularExpressions;

using ArchUnitNET.Loader;
using ArchUnitNET.xUnitV3;

using Holefeeder.Service;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Holefeeder.ServiceTests.Shared;

public class ArchitectureRules
{
    private const string Root = @"^Holefeeder\.Service\.";
    private const string NamespaceEnd = @"(\.|$)";

    private static readonly string[] OuterRings = ["Api", "Events", "Jobs", "Persistence", "Ui"];

    private static readonly ArchUnitNET.Domain.Architecture Service =
        new ArchLoader().LoadAssemblies(typeof(Holefeeder.Service.Api).Assembly).Build();

    private static readonly string[] CrossCutting = ["Shared", "Support"];

    private static readonly string[] Features = Service.Types
        .Select(type => Regex.Match(type.Namespace.FullName,
            $@"{Root}(?<feature>[^.]+)\.(Core|{string.Join('|', OuterRings)}){NamespaceEnd}"))
        .Where(match => match.Success && !CrossCutting.Contains(match.Groups["feature"].Value))
        .Select(match => match.Groups["feature"].Value)
        .Distinct(StringComparer.Ordinal)
        .Order(StringComparer.Ordinal)
        .ToArray();

    private static readonly string FeatureOwner = $"({string.Join('|', Features)})";

    private static readonly string CrossCuttingOwner = $"({string.Join('|', CrossCutting)})";

    private static readonly string NewCodeOwner = $"({string.Join('|', [.. Features, .. CrossCutting])})";

    private static readonly string RingOwner = $@"{NewCodeOwner}\.";

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

    [Fact]
    public void FeaturesAreDiscovered() =>
        Features.Should().Contain(["Home", "StoreItems"]).And.NotContain(CrossCutting);

    [Theory]
    [MemberData(nameof(LegacyNamespaces))]
    public void NewCodeDoesNotUseLegacyNamespaces(string legacyNamespace) =>
        Types().That().ResideInNamespaceMatching($"{Root}{NewCodeOwner}{NamespaceEnd}")
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
    public void CrossCuttingDoesNotDependOnFeatures() =>
        Types().That().ResideInNamespaceMatching($"{Root}{CrossCuttingOwner}{NamespaceEnd}")
            .Should().NotDependOnAnyTypesThat()
            .ResideInNamespaceMatching($"{Root}{FeatureOwner}{NamespaceEnd}")
            .WithoutRequiringPositiveResults()
            .Check(Service);

    [Fact]
    public void FeatureDoesNotDependOnAnotherFeature()
    {
        foreach (var feature in Features)
        {
            var otherFeatures = string.Join('|', Features.Where(other => other != feature));
            Types().That().ResideInNamespaceMatching($"{Root}{feature}{NamespaceEnd}")
                .Should().NotDependOnAnyTypesThat()
                .ResideInNamespaceMatching($"{Root}({otherFeatures}){NamespaceEnd}")
                .Check(Service);
        }
    }
}
