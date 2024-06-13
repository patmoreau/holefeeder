// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using DrifterApps.Seeds.Application;

using Holefeeder.Application.Context;
using Holefeeder.Application.UserContext;

using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Tags.Queries;

public partial class GetTagsWithCount
{
    internal class Handler(IUserContext userContext, BudgetingContext context) : IRequestHandler<Request, IEnumerable<TagDto>>
    {
        private readonly BudgetingContext _context = context;
        private readonly IUserContext _userContext = userContext;

        public async Task<IEnumerable<TagDto>> Handle(Request request, CancellationToken cancellationToken)
        {
            var transactions = await _context.Transactions
                .Where(transaction => transaction.UserId == _userContext.Id)
                .ToListAsync(cancellationToken: cancellationToken);

            var results = transactions.SelectMany(transaction => transaction.Tags,
                    (_, tag) => new { Tag = tag })
                .GroupBy(x => new { x.Tag })
                .Select(group => new TagDto(group.Key.Tag, group.Count()))
                .OrderByDescending(x => x.Count);

            return results;
        }
    }
}
