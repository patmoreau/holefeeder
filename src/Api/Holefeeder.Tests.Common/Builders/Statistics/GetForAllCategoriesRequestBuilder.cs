// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using static Holefeeder.Application.Features.Statistics.Queries.GetForAllCategories;

namespace Holefeeder.Tests.Common.Builders.Statistics;

internal class GetForAllCategoriesRequestBuilder : IBuilder<Request>
{
    private readonly Faker<Request> _faker = new AutoFaker<Request>();

    public Request Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }
}
