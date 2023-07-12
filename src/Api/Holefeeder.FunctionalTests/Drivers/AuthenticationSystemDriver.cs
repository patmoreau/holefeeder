using System.Diagnostics;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Holefeeder.FunctionalTests.Drivers;

public class AuthenticationSystemDriver
{
    public const string ValidToken = nameof(ValidToken);

    private WireMockServer? _azureB2C;

    public void Start()
    {
        if (_azureB2C is not { IsStarted: true })
        {
            _azureB2C = StartWithRetry(9999);
        }
    }

    public void Stop()
    {
        if (_azureB2C is not { IsStarted: true })
        {
            return;
        }

        _azureB2C.Stop();
        _azureB2C.Dispose();
    }

    public void SetupValidToken() => SetToken(ValidToken);

    public void SetupInvalidToken() => SetToken(string.Empty);

    private void SetToken(string accessToken)
    {
        Debug.Assert(_azureB2C != null, nameof(_azureB2C) + " != null");
        string token = "{\"access_token\": " +
                       $"\"{accessToken}\"," +
                       "\"expires_in\": 900," +
                       "\"token_type\": \"Bearer\"," +
                       "\"scope\": \"holefeeder.user\"}";
        _azureB2C.Given(Request.Create()).RespondWith(Response.Create().WithBody(token));
    }

    private static WireMockServer StartWithRetry(int port, int retryCount = 5)
    {
        while (retryCount > 0)
        {
            try
            {
                return WireMockServer.Start(port);
            }
#pragma warning disable CA1031
            catch (Exception)
#pragma warning restore CA1031
            {
                Thread.Sleep(250);
                retryCount--;
            }
        }

        throw new InvalidOperationException($"Could not instantiate WireMockServer on port {port}");
    }
}
