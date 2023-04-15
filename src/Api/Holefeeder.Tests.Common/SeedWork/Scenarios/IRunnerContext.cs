// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace Holefeeder.Tests.Common.SeedWork.Scenarios;

public interface IRunnerContext
{
    void SetContextData(string contextKey, object data);
    public T GetContextData<T>(string contextKey);
}
