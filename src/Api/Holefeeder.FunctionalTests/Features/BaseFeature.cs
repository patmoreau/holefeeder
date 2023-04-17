// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Bogus;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.StepDefinitions;
using Holefeeder.Tests.Common.SeedWork.Drivers;
using LightBDD.XUnit2;

namespace Holefeeder.FunctionalTests.Features;

[Collection("Api collection")]
public class BaseFeature : FeatureFixture
{
    private readonly Faker _faker = new();
    protected HttpClientDriver HttpClientDriver { get; }

    protected BaseFeature(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
        if (apiApplicationDriver == null)
        {
            throw new ArgumentNullException(nameof(apiApplicationDriver));
        }

        // Scope = apiApplicationDriver.Services.CreateScope();

        HttpClientDriver = apiApplicationDriver.CreateHttpClientDriver(testOutputHelper);
    }

    protected Task Given_an_unauthorized_user() => Task.Run(() => HttpClientDriver.UnAuthenticate());

    protected Task Given_an_authorized_user() => Task.Run(() =>
        HttpClientDriver.AuthenticateUser(UserStepDefinition.HolefeederUserId));

    protected Task Given_a_forbidden_user() =>
        Task.Run(() => HttpClientDriver.AuthenticateUser(_faker.Random.Guid()));
}
