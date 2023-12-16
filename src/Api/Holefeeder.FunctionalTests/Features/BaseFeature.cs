// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Bogus;
using DrifterApps.Seeds.Testing.Drivers;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.StepDefinitions;
using LightBDD.XUnit2;

namespace Holefeeder.FunctionalTests.Features;

[Collection("Api collection")]
public class BaseFeature : FeatureFixture
{
    private readonly Faker _faker = new();
    protected IHttpClientDriver HttpClientDriver { get; }

    protected BaseFeature(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
        ArgumentNullException.ThrowIfNull(apiApplicationDriver);
        HttpClientDriver = apiApplicationDriver.CreateHttpClientDriver(testOutputHelper);
    }

    protected Task Given_an_unauthorized_user() => Task.Run(() => HttpClientDriver.UnAuthenticate());

    protected Task Given_an_authorized_user() => Task.Run(() =>
        HttpClientDriver.AuthenticateUser(UserStepDefinition.HolefeederUserId.ToString()));

    protected Task Given_a_forbidden_user() =>
        Task.Run(() => HttpClientDriver.AuthenticateUser(_faker.Random.Hash()));
}
