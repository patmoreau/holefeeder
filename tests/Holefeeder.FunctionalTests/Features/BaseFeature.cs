// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using DrifterApps.Seeds.Testing.Drivers;

using Holefeeder.FunctionalTests.Drivers;

using LightBDD.XUnit2;

namespace Holefeeder.FunctionalTests.Features;

[Collection("Api Security collection")]
public class BaseFeature : FeatureFixture
{
    protected IHttpClientDriver HttpClientDriver { get; }

    protected BaseFeature(ApiApplicationSecurityDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
        ArgumentNullException.ThrowIfNull(apiApplicationDriver);
        HttpClientDriver = apiApplicationDriver.CreateHttpClientDriver(testOutputHelper);
    }

    protected Task Given_an_unauthorized_user() => Task.Run(() => HttpClientDriver.UnAuthenticate());

    protected Task Given_an_authorized_user() => Task.Run(() =>
        HttpClientDriver.AuthenticateUser(TestUsers[AuthorizedUser].IdentityObjectId));

    protected Task Given_a_forbidden_user() =>
        Task.Run(() => HttpClientDriver.AuthenticateUser(TestUsers["ForbiddenUser"].IdentityObjectId));
}
