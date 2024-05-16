// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace Holefeeder.Application.Features.Statistics.Queries;

public partial class GetSummary
{
    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.From).NotEmpty();
            RuleFor(command => command.To).NotEmpty();
        }
    }
}
