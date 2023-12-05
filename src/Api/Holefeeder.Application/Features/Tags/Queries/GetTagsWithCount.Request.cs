// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace Holefeeder.Application.Features.Tags.Queries;

public partial class GetTagsWithCount
{
    internal partial record Request() : IRequest<IEnumerable<TagDto>>;
}
