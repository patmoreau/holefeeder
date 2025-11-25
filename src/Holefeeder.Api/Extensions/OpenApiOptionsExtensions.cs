using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Holefeeder.Api.Extensions;

internal static class OpenApiOptionsExtensions
{
    public static OpenApiOptions AddBearerTokenAuthentication(this OpenApiOptions options)
    {
        var scheme = new OpenApiSecurityScheme()
        {
            Type = SecuritySchemeType.Http,
            Name = IdentityConstants.BearerScheme,
            Scheme = "Bearer"
        };

        options.AddDocumentTransformer((document, _, _) =>
        {
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes!.Add(IdentityConstants.BearerScheme, scheme);
            return Task.CompletedTask;
        });
        options.AddOperationTransformer((operation, context, _) =>
        {
            if (context.Description.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>().Any())
            {
                var schemeReference = new OpenApiSecuritySchemeReference(IdentityConstants.BearerScheme);
                operation.Security = [new() { [schemeReference] = [] }];
            }
            return Task.CompletedTask;
        });
        return options;
    }

}
