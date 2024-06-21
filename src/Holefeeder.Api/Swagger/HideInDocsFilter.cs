using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Holefeeder.Api.Swagger;
internal class HideInDocsFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context) =>
        swaggerDoc.Paths.Remove("/");
}
