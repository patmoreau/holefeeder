// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using System.Reflection;

using Microsoft.AspNetCore.Http;

namespace Holefeeder.Application.Features.Statistics.Queries;

public partial class GetSummary
{
    internal record Request(DateOnly From, DateOnly To) : IRequest<SummaryDto>
    {
        public static ValueTask<Request?> BindAsync(HttpContext context, ParameterInfo parameter)
        {
            const string fromKey = "from";
            const string toKey = "to";

            var hasFromDate = DateOnly.TryParse(context.Request.Query[fromKey], CultureInfo.InvariantCulture, out var from);
            var hasToDate = DateOnly.TryParse(context.Request.Query[toKey], CultureInfo.InvariantCulture, out var to);

            Request result = new(hasFromDate ? from : default, hasToDate ? to : default);

            return ValueTask.FromResult<Request?>(result);
        }
    }
}
