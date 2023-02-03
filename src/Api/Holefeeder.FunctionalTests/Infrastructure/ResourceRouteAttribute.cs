using System.Globalization;
using System.Reflection;

namespace Holefeeder.FunctionalTests.Infrastructure;

[AttributeUsage(AttributeTargets.Field)]
public sealed class ResourceRouteAttribute : Attribute
{
    public ResourceRouteAttribute(string endpoint)
    {
        Endpoint = endpoint;
    }

    public string Endpoint { get; }

    internal static Uri EndpointFromResource(ApiResources resource)
    {
        var fieldInfo = typeof(ApiResources).GetField(resource.ToString())!;
        var attribute = fieldInfo.GetCustomAttribute<ResourceRouteAttribute>()!;
        return new Uri(attribute.Endpoint, UriKind.Relative);
    }

    internal static Uri EndpointFromResource(ApiResources resource, params object?[] parameters)
    {
        var fieldInfo = typeof(ApiResources).GetField(resource.ToString())!;
        var attribute = fieldInfo.GetCustomAttribute<ResourceRouteAttribute>()!;
        return new Uri(string.Format(CultureInfo.InvariantCulture, attribute.Endpoint, parameters), UriKind.Relative);
    }
}
