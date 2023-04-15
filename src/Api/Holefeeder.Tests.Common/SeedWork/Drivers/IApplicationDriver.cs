// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Xunit.Abstractions;

namespace Holefeeder.Tests.Common.SeedWork.Drivers;

public interface IApplicationDriver
{
    IServiceProvider Services { get; }

    HttpClientDriver CreateHttpClientDriver(ITestOutputHelper testOutputHelper);
}
