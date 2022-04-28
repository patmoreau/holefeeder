using System.Diagnostics.CodeAnalysis;

using Holefeeder.Application.SeedWork;

using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Holefeeder.Api.Swagger;

// ReSharper disable once ClassNeverInstantiated.Global
[ExcludeFromCodeCoverage]
public class QueryRequestOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        if (context.ApiDescription.ActionDescriptor is not { } descriptor ||
            !descriptor.EndpointMetadata.Any(metaData => metaData is nameof(IRequestQuery)))
        {
            return;
        }

        foreach (var properties in typeof(IRequestQuery).GetProperties())
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = properties.Name.ToLower(),
                In = ParameterLocation.Query,
                Required = false,
                Schema = new OpenApiSchema {Type = properties.PropertyType.Name}
            });
        }
    }
}
