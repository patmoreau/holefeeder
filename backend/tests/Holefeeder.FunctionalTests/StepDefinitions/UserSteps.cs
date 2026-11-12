using System.Net;

using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.StepDefinitions;

using Holefeeder.Application;
using Holefeeder.Application.Features.Accounts.Commands;
using Holefeeder.Application.Features.MyData.Commands;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Application.Features.StoreItems.Commands;
using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.Application.Features.Transactions.Commands;
using Holefeeder.Application.Models;
using Holefeeder.Application.UseCases;
using Holefeeder.FunctionalTests.Infrastructure;

using Refit;

namespace Holefeeder.FunctionalTests.StepDefinitions;

internal sealed partial class UserSteps(IApplicationDriver applicationDriver) : ApiSteps<IUser>(applicationDriver)
{
    internal void ClosesTheAccount(IStepRunner runner) =>
        runner.Execute<CloseAccount.Request, IApiResponse>(request =>
        {
            request.Should().BeValid();
            return Api.CloseAccountAsync(request);
        });

    internal void SetsAFavoriteAccount(IStepRunner runner) =>
        runner.Execute<FavoriteAccount.Request, IApiResponse>(request =>
        {
            request.Should().BeValid();
            return Api.FavoriteAccountAsync(request);
        });

    internal void ModifiesAnAccount(IStepRunner runner) =>
        runner.Execute<ModifyAccount.Request, IApiResponse>(request =>
        {
            request.Should().BeValid();
            return Api.ModifyAccountAsync(request.Value);
        });

    internal void OpensAnAccount(IStepRunner runner) =>
        runner.Execute<OpenAccount.Request, IApiResponse>(request =>
        {
            request.Should().BeValid();
            return Api.OpenAccountAsync(request.Value);
        });

    internal void GetsTheCurrentUser(IStepRunner runner) => runner.Execute(() => Api.GetCurrentUserAsync());

    internal void Registers(IStepRunner runner) => runner.Execute(() => Api.RegisterUserAsync());

    internal void ExportsTheirData(IStepRunner runner) => runner.Execute(() => Api.ExportDataAsync());

    internal void ImportsTheirData(IStepRunner runner) =>
        runner.Execute<ImportData.Request, IApiResponse>(request =>
        {
            request.Should().BeValid();
            return Api.ImportDataAsync(request.Value);
        });

    internal void WaitsForImportCompletion(IStepRunner runner) =>
        runner.Execute<Guid, ImportDataStatusDto?>(async id =>
        {
            id.Should().BeValid();

            var tries = 0;
            const int retryDelayInSeconds = 50;
            const int numberOfRetry = 10;

            while (tries < numberOfRetry)
            {
                var response = await Api.ImportDataStatusAsync(id);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    await Task.Delay(TimeSpan.FromSeconds(retryDelayInSeconds));
                    tries++;
                    continue;
                }

                response.Should().BeSuccessful().And.HaveStatusCode(HttpStatusCode.OK);

                var dto = response.Content;
                dto.Should().NotBeNull();
                if (dto!.Status == CommandStatus.Completed)
                {
                    return dto;
                }

                if (dto.Status == CommandStatus.Error)
                {
                    return dto;
                }

                await Task.Delay(TimeSpan.FromSeconds(retryDelayInSeconds));

                tries++;
            }

            return null;
        });

    internal void GetDashboardSummary(IStepRunner runner) => runner.Execute(Api.GetDashboardSummary);

    internal void CreatesAnItemInTheStore(IStepRunner runner) =>
        runner.Execute<CreateStoreItem.Request, IApiResponse>(request =>
        {
            request.Should().BeValid();
            return Api.CreateStoreItemAsync(request.Value);
        });

    internal void ModifiesAnItemInTheStore(IStepRunner runner) =>
        runner.Execute<ModifyStoreItem.Request, IApiResponse>(request =>
        {
            request.Should().BeValid();
            return Api.ModifyStoreItemAsync(request.Value);
        });

    internal void GetsAnItemInTheStore(IStepRunner runner) =>
        runner.Execute<Guid, IApiResponse<StoreItemViewModel>>(async id =>
        {
            id.Should().BeValid();
            var response = await Api.GetStoreItemAsync(id.Value);
            return response;
        });

    internal void GetsItemsInTheStore(IStepRunner runner) =>
        runner.Execute<GetStoreItems.Request, IApiResponse<IEnumerable<StoreItemViewModel>>>(async request =>
        {
            request.Should().BeValid();
            var query = request.Value;
            var response = await Api.GetStoreItemsAsync(query.Offset, query.Limit, query.Sort, query.Filter);
            return response;
        });

    internal void ModifiesACashflow(IStepRunner runner) =>
        runner.Execute<ModifyCashflow.Request, IApiResponse>(request =>
        {
            request.Should().BeValid();
            return Api.ModifyCashflowAsync(request.Value);
        });

    internal void DeletesATransaction(IStepRunner runner) =>
        runner.Execute<DeleteTransaction.Request, IApiResponse>(request =>
        {
            request.Should().BeValid();
            return Api.DeleteTransactionAsync(request.Value.Id);
        });

    internal void ModifiesATransaction(IStepRunner runner) =>
        runner.Execute<ModifyTransaction.Request, IApiResponse>(request =>
        {
            request.Should().BeValid();
            return Api.ModifyTransactionAsync(request.Value);
        });

    internal void GetsATransaction(IStepRunner runner) =>
        runner.Execute<Guid, IApiResponse<TransactionInfoViewModel>>(id =>
        {
            id.Should().BeValid();
            return Api.GetTransactionAsync(id.Value);
        });

    internal void SyncTheirData(IStepRunner runner) =>
        runner.Execute<PowerSync.Request, IApiResponse>(request =>
        {
            request.Should().BeValid();
            return Api.SyncAsync(request.Value);
        });
}
