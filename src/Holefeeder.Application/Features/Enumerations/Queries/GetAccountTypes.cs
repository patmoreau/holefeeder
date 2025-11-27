using Holefeeder.Domain.Features.Accounts;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.Enumerations.Queries;

public class GetAccountTypes : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/enumerations/get-account-types",
                async (CancellationToken cancellationToken) =>
                {
                    var result = await Handle(cancellationToken);
                    return Results.Ok(result);
                })
            .Produces<IEnumerable<AccountType>>()
            .WithTags(nameof(Enumerations))
            .WithName(nameof(GetAccountTypes));

    private static Task<IReadOnlyCollection<AccountType>> Handle(CancellationToken cancellationToken) =>
        Task.FromResult(AccountType.List);
}
