using System.Globalization;
using System.Reflection;

namespace Holefeeder.FunctionalTests.Infrastructure;

[AttributeUsage(AttributeTargets.Field)]
public sealed class ResourceRouteAttribute : Attribute
{
    public string Endpoint { get; }

    public ResourceRouteAttribute(string endpoint)
    {
        Endpoint = endpoint;
    }

    internal static Uri EndpointFromResource(ApiResources resource)
    {
        FieldInfo fieldInfo = typeof(ApiResources).GetField(resource.ToString())!;
        ResourceRouteAttribute attribute = fieldInfo.GetCustomAttribute<ResourceRouteAttribute>()!;
        return new Uri(attribute.Endpoint, UriKind.Relative);
    }

    internal static Uri EndpointFromResource(ApiResources resource, params object?[] parameters)
    {
        FieldInfo fieldInfo = typeof(ApiResources).GetField(resource.ToString())!;
        ResourceRouteAttribute attribute = fieldInfo.GetCustomAttribute<ResourceRouteAttribute>()!;
        return new Uri(string.Format(CultureInfo.InvariantCulture, attribute.Endpoint, parameters), UriKind.Relative);
    }
}
