// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Reflection;

using Microsoft.AspNetCore.Http;

namespace Holefeeder.Application.Features.Statistics.Queries;

public partial class GetSummary
{
    internal record Request(DateOnly AsOf) : IRequest<SummaryDto>
    {
        public static ValueTask<Request?> BindAsync(HttpContext context, ParameterInfo parameter)
        {
            const string asOfKey = "as-of";

            var hasAsOfDate = DateOnly.TryParse(context.Request.Query[asOfKey], out var asOf);

            Request result = new(hasAsOfDate ? asOf : DateOnly.FromDateTime(DateTime.Today));

            return ValueTask.FromResult<Request?>(result);
        }
    }
}
