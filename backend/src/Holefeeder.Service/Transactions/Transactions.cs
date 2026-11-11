using Holefeeder.Service.Transactions.Api.GetNewTransactionOptions;
using Holefeeder.Service.Transactions.Api.GetTagSuggestions;
using Holefeeder.Service.Transactions.Core;
using Holefeeder.Service.Transactions.Core.GetNewTransactionOptions;
using Holefeeder.Service.Transactions.Core.GetTagSuggestions;
using Holefeeder.Service.Transactions.Persistence;

namespace Holefeeder.Service.Transactions;

internal static class Transactions
{
    public static IServiceCollection AddTransactionsServices(this IServiceCollection services) =>
        services
            .AddScoped<ITransactions, TransactionsInDatabase>()
            .AddScoped<GetNewTransactionOptionsUseCase>()
            .AddScoped<GetTagSuggestionsUseCase>();

    public static IEndpointRouteBuilder MapTransactionsEndpoints(this IEndpointRouteBuilder app) =>
        app.MapGetNewTransactionOptions()
            .MapGetTagSuggestions();
}
