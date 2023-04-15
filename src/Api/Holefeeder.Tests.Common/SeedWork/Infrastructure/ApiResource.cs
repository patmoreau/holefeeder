using System.Globalization;
using System.Text.RegularExpressions;

namespace Holefeeder.Tests.Common.SeedWork.Infrastructure;

public partial class ApiResource
{
    public static ApiResource DefineApi(string endpointTemplate, HttpMethod httpMethod) =>
        new(endpointTemplate, httpMethod);

    public static ApiResource DefineOpenApi(string endpointTemplate, HttpMethod httpMethod) =>
        new OpenApiResource(endpointTemplate, httpMethod);

    public string EndpointTemplate { get; }

    public virtual bool IsOpen => false;

    public HttpMethod HttpMethod { get; }

    public int ParameterCount => ParametersRegex().Matches(EndpointTemplate).Count;

    private ApiResource(string endpointTemplate, HttpMethod httpMethod)
    {
        EndpointTemplate = endpointTemplate;
        HttpMethod = httpMethod;
    }

    private sealed class OpenApiResource : ApiResource
    {
        public OpenApiResource(string endpointTemplate, HttpMethod httpMethod) : base(endpointTemplate, httpMethod)
        {
        }

        public override bool IsOpen => true;
    }

    internal Uri EndpointFromResource() => new(EndpointTemplate, UriKind.Relative);

    internal Uri EndpointFromResource(params object[] parameters) =>
        new(string.Format(CultureInfo.InvariantCulture, EndpointTemplate, parameters), UriKind.Relative);

    [GeneratedRegex("\\{\\d+\\}")]
    private static partial Regex ParametersRegex();
}
