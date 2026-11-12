using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;

using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit.Persistence;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

using Npgsql;

namespace Holefeeder.ServiceTests.TestKit.Api;

public sealed class ServerForTest : WebApplicationFactory<Service.Api>, IAsyncLifetime
{
    private const string ConnectionStringKey = "ConnectionStrings:BudgetingConnectionString";
    private const string AuthenticationScheme = "Auth0";
    private const string Issuer = "https://holefeeder-service-tests.localtest.me/";
    private const string Audience = "https://holefeeder-api.drifterapps.app";
    private const string UserScopes = "read:user write:user";
    private const string UnreachableConnectionString = "Host=127.0.0.1;Port=1;Username=nobody;Database=nothing;Timeout=1";

    private readonly RSA _signingAlgorithm = RSA.Create(2048);
    private readonly RsaSecurityKey _signingKey;

    public ServerForTest()
    {
        _signingKey = new RsaSecurityKey(_signingAlgorithm) { KeyId = Guid.NewGuid().ToString() };
    }

    public DatabaseForTest Database { get; } = new();

    internal HttpClient CreateClientFor(IdentityObjectId identityObjectId) =>
        CreateClientFor(identityObjectId, UserScopes);

    internal HttpClient CreateClientFor(IdentityObjectId identityObjectId, string scopes)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", CreateAccessToken(identityObjectId.Value, scopes));
        return client;
    }

    public HttpClient CreateClientWithUnreachableDatabaseFor(string identityObjectId)
    {
        var client = WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<NpgsqlDataSource>();
            services.AddSingleton(_ => NpgsqlDataSource.Create(UnreachableConnectionString));
        })).CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", CreateAccessToken(identityObjectId, UserScopes));
        return client;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.UseEnvironment("SERVICE_TESTS");
        builder.UseSetting(ConnectionStringKey, Database.ConnectionString);
        builder.ConfigureTestServices(services =>
            services.Configure<JwtBearerOptions>(AuthenticationScheme, options =>
            {
                options.MetadataAddress = string.Empty;
                options.Configuration = new OpenIdConnectConfiguration { Issuer = Issuer };
                options.Configuration.SigningKeys.Add(_signingKey);
            }));
    }

    private string CreateAccessToken(string identityObjectId, string scopes) =>
        new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Issuer = Issuer,
            Audience = Audience,
            Subject = new ClaimsIdentity([new Claim("sub", identityObjectId), new Claim("scope", scopes)]),
            SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.RsaSha256)
        });

    public async ValueTask InitializeAsync()
    {
        await Database.InitializeAsync();
        _ = Services;
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await Database.DisposeAsync();
        _signingAlgorithm.Dispose();
    }
}
