using System.Globalization;
using System.Text.RegularExpressions;
using Ardalis.SmartEnum;

namespace Holefeeder.FunctionalTests.Infrastructure;

public partial class ApiResource : SmartEnum<ApiResource>
{
    public static readonly ApiResource GetAccounts = new(nameof(GetAccounts), 101, "api/v2/accounts", HttpMethod.Get);
    public static readonly ApiResource GetAccount = new(nameof(GetAccount), 102, "api/v2/accounts/{0}", HttpMethod.Get);
    public static readonly ApiResource CloseAccount = new(nameof(CloseAccount), 103, "api/v2/accounts/close-account", HttpMethod.Post);
    public static readonly ApiResource ModifyAccount = new(nameof(ModifyAccount), 104, "api/v2/accounts/modify-account", HttpMethod.Post);
    public static readonly ApiResource OpenAccount = new(nameof(OpenAccount), 105, "api/v2/accounts/open-account", HttpMethod.Post);
    public static readonly ApiResource FavoriteAccount = new(nameof(FavoriteAccount), 106, "api/v2/accounts/favorite-account", HttpMethod.Post);

    public static readonly ApiResource GetCashflows = new(nameof(GetCashflows), 201, "api/v2/cashflows", HttpMethod.Get);
    public static readonly ApiResource GetCashflow = new(nameof(GetCashflow), 202, "api/v2/cashflows/{0}", HttpMethod.Get);
    public static readonly ApiResource ModifyCashflow = new(nameof(ModifyCashflow), 203, "api/v2/cashflows/modify", HttpMethod.Post);
    public static readonly ApiResource CancelCashflow = new(nameof(CancelCashflow), 204, "api/v2/cashflows/cancel", HttpMethod.Post);
    public static readonly ApiResource GetUpcoming = new(nameof(GetUpcoming), 205, "api/v2/cashflows/get-upcoming?from={0}&to={1}", HttpMethod.Get);

    public static readonly ApiResource GetCategories = new(nameof(GetCategories), 300, "api/v2/categories", HttpMethod.Get);

    public static readonly ApiResource GetAccountTypes = new OpenApiResource(nameof(GetAccountTypes), 401, "api/v2/enumerations/get-account-types", HttpMethod.Get);
    public static readonly ApiResource GetCategoryTypes = new OpenApiResource(nameof(GetCategoryTypes), 402, "api/v2/enumerations/get-category-types", HttpMethod.Get);
    public static readonly ApiResource GetDateIntervalTypes = new OpenApiResource(nameof(GetDateIntervalTypes), 403, "api/v2/enumerations/get-date-interval-types", HttpMethod.Get);

    public static readonly ApiResource ExportData = new(nameof(ExportData), 501, "api/v2/my-data/export-data", HttpMethod.Get);
    public static readonly ApiResource ImportData = new(nameof(ImportData), 502, "api/v2/my-data/import-data", HttpMethod.Post);
    public static readonly ApiResource ImportDataStatus = new(nameof(ImportDataStatus), 503, "api/v2/my-data/import-status/{0}", HttpMethod.Get);

    public static readonly ApiResource GetStoreItems = new(nameof(GetStoreItems), 601, "api/v2/store-items", HttpMethod.Get);
    public static readonly ApiResource GetStoreItem = new(nameof(GetStoreItem), 602, "api/v2/store-items/{0}", HttpMethod.Get);
    public static readonly ApiResource CreateStoreItem = new(nameof(CreateStoreItem), 603, "api/v2/store-items/create-store-item", HttpMethod.Post);
    public static readonly ApiResource ModifyStoreItem = new(nameof(ModifyStoreItem), 604, "api/v2/store-items/modify-store-item", HttpMethod.Post);

    public static readonly ApiResource MakePurchase = new(nameof(MakePurchase), 701, "api/v2/transactions/make-purchase", HttpMethod.Post);
    public static readonly ApiResource PayCashflow = new(nameof(PayCashflow), 702, "api/v2/transactions/pay-cashflow", HttpMethod.Post);
    public static readonly ApiResource Transfer = new(nameof(Transfer), 703, "api/v2/transactions/transfer", HttpMethod.Post);
    public static readonly ApiResource ModifyTransaction = new(nameof(ModifyTransaction), 704, "api/v2/transactions/modify", HttpMethod.Post);
    public static readonly ApiResource DeleteTransaction = new(nameof(DeleteTransaction), 705, "api/v2/transactions/{0}", HttpMethod.Delete);
    public static readonly ApiResource GetTransaction = new(nameof(GetTransaction), 706, "api/v2/transactions/{0}", HttpMethod.Get);
    public static readonly ApiResource GetTransactions = new(nameof(GetTransactions), 707, "api/v2/transactions", HttpMethod.Get);

    public static readonly ApiResource GetForAllCategories = new(nameof(GetForAllCategories), 800, "api/v2/categories/statistics", HttpMethod.Get);
    public string Endpoint { get; }

    public virtual bool IsOpen => false;

    public HttpMethod HttpMethod { get; }

    public int ParameterCount => ParametersRegex().Matches(Endpoint).Count;

    private ApiResource(string name, int value, string endpoint, HttpMethod httpMethod) : base(name, value)
    {
        Endpoint = endpoint;
        HttpMethod = httpMethod;
    }

    private sealed class OpenApiResource : ApiResource
    {
        public OpenApiResource(string name, int value, string endpoint, HttpMethod httpMethod)
            : base(name, value, endpoint, httpMethod)
        {
        }

        public override bool IsOpen => true;
    }

    internal Uri EndpointFromResource() => new(Endpoint, UriKind.Relative);

    internal Uri EndpointFromResource(params object[] parameters) =>
        new(string.Format(CultureInfo.InvariantCulture, Endpoint, parameters), UriKind.Relative);

    [GeneratedRegex("\\{\\d+\\}")]
    private static partial Regex ParametersRegex();
}
