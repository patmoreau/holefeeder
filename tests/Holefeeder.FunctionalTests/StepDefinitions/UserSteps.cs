using System.Net;

using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.StepDefinitions;

using Holefeeder.Application;
using Holefeeder.Application.Features.Accounts.Commands;
using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.Application.Features.MyData.Commands;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Application.Features.Statistics.Queries;
using Holefeeder.Application.Features.StoreItems.Commands;
using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.Application.Features.Tags.Queries;
using Holefeeder.FunctionalTests.Infrastructure;

using Refit;

using AccountViewModel = Holefeeder.Application.Features.Accounts.Queries.AccountViewModel;

namespace Holefeeder.FunctionalTests.StepDefinitions;

public class UserSteps(IApplicationDriver applicationDriver) : ApiSteps<IUser>(applicationDriver)
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

    internal void GetsAnAccount(IStepRunner runner) =>
        runner.Execute<Guid, IApiResponse<AccountViewModel>>(async id =>
        {
            id.Should().BeValid();
            var response = await Api.GetAccountAsync(id.Value);
            return response;
        });

    internal void GetsAccounts(IStepRunner runner) =>
        runner.Execute<GetAccounts.Request, IApiResponse<IEnumerable<AccountViewModel>>>(async request =>
        {
            request.Should().BeValid();
            var query = request.Value;
            var response = await Api.GetAccountsAsync(query.Offset, query.Limit, query.Sort, query.Filter);
            return response;
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

    internal void GetsCategories(IStepRunner runner) => runner.Execute(() => Api.GetCategoriesAsync());

    internal void GetsTheListOfAccountTypes(IStepRunner runner) => runner.Execute(() => Api.GetAccountTypesAsync());

    internal void GetsTheListOfCategoryTypes(IStepRunner runner) => runner.Execute(() => Api.GetCategoryTypesAsync());

    internal void GetsTheListOfDateIntervalTypes(IStepRunner runner) =>
        runner.Execute(() => Api.GetDateIntervalTypesAsync());

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

                if(response.StatusCode == HttpStatusCode.NotFound)
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

    internal void GetsTheirTags(IStepRunner runner) => runner.Execute(() => Api.GetTagsWithCountAsync());

    internal void GetSummaryStatisticsForRange(IStepRunner runner, DateOnly from, DateOnly until) =>
        runner.Execute(() => Api.GetStatisticsSummaryAsync(from, until));

    internal void GetsTheirStatistics(IStepRunner runner) =>
        runner.Execute(() => Api.GetStatisticsForAllCategoriesAsync());

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
}
