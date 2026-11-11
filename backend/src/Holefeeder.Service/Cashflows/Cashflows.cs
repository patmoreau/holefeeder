using Holefeeder.Service.Cashflows.Api.ClearCashflow;
using Holefeeder.Service.Cashflows.Api.PayCashflow;
using Holefeeder.Service.Cashflows.Core;
using Holefeeder.Service.Cashflows.Core.ClearCashflow;
using Holefeeder.Service.Cashflows.Core.PayCashflow;
using Holefeeder.Service.Cashflows.Persistence;

namespace Holefeeder.Service.Cashflows;

internal static class Cashflows
{
    public static IServiceCollection AddCashflowsServices(this IServiceCollection services) =>
        services
            .AddScoped<ICashflows, CashflowsInDatabase>()
            .AddScoped<PayCashflowUseCase>()
            .AddScoped<ClearCashflowUseCase>();

    public static IEndpointRouteBuilder MapCashflowsEndpoints(this IEndpointRouteBuilder app) =>
        app.MapPayCashflow()
            .MapClearCashflow();
}
