// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace Holefeeder.Tests.Common.SeedWork;

internal abstract class RootBuilder<T> where T : class
{
    protected virtual Faker<T> Faker { get; } = new AutoFaker<T>();

    public virtual T Build()
    {
        Faker.AssertConfigurationIsValid();
        return Faker.Generate();
    }
    public virtual IReadOnlyCollection<T> BuildCollection(int? count = null)
    {
        Faker.AssertConfigurationIsValid();
        return Faker.Generate(count ?? Globals.RandomCollectionCount());
    }
}
