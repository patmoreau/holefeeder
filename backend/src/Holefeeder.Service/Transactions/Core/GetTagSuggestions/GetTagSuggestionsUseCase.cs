using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Transactions.Core.GetTagSuggestions;

internal sealed class GetTagSuggestionsUseCase(ITransactions transactions)
{
    public async Task<Result<IReadOnlyList<string>>> HandleAsync(GetTagSuggestionsRequest request, CancellationToken cancellationToken)
    {
        var usages = await transactions.GetTagUsagesAsync(request.User, cancellationToken);
        if (usages.IsFailure)
        {
            return usages.Error;
        }

        return usages.Value
            .GroupBy(usage => usage.Tag)
            .Select(tag => new
            {
                Tag = tag.Key,
                WithCategory = tag.Where(usage => usage.CategoryId == request.CategoryId).Sum(usage => usage.Count),
                Overall = tag.Sum(usage => usage.Count)
            })
            .OrderByDescending(tag => tag.WithCategory)
            .ThenByDescending(tag => tag.Overall)
            .ThenBy(tag => tag.Tag, StringComparer.Ordinal)
            .Select(tag => tag.Tag)
            .ToList();
    }
}
