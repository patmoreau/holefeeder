using System.Diagnostics.CodeAnalysis;

using DrifterApps.Seeds.Application;

using Microsoft.OpenApi;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Holefeeder.Api.Swagger;

// ReSharper disable once ClassNeverInstantiated.Global
[ExcludeFromCodeCoverage]
internal class QueryRequestOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(context);

        operation.Parameters ??= [];

        if (context.ApiDescription.ActionDescriptor is not { } descriptor ||
            !descriptor.EndpointMetadata.Any(metaData => metaData is nameof(IRequestQuery)))
        {
            return;
        }

        foreach (var properties in typeof(IRequestQuery).GetProperties())
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = properties.Name.ToLowerInvariant(),
                In = ParameterLocation.Query,
                Required = false,
                Schema = new OpenApiSchema { Type = JsonSchemaType.String }
            });
        }
    }
}
