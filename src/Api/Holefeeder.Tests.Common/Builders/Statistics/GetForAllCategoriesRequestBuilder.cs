// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using DrifterApps.Seeds.Testing;
using static Holefeeder.Application.Features.Statistics.Queries.GetForAllCategories;

namespace Holefeeder.Tests.Common.Builders.Statistics;

internal class GetForAllCategoriesRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = new();
}
