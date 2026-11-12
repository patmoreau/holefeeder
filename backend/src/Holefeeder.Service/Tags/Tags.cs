using Holefeeder.Service.Tags.Api.ListTags;
using Holefeeder.Service.Tags.Api.RenameTag;
using Holefeeder.Service.Tags.Core;
using Holefeeder.Service.Tags.Core.ListTags;
using Holefeeder.Service.Tags.Core.RenameTag;
using Holefeeder.Service.Tags.Persistence;

namespace Holefeeder.Service.Tags;

internal static class Tags
{
    public static IServiceCollection AddTagsServices(this IServiceCollection services) =>
        services
            .AddScoped<ITags, TagsInDatabase>()
            .AddScoped<ListTagsUseCase>()
            .AddScoped<RenameTagUseCase>();

    public static IEndpointRouteBuilder MapTagsEndpoints(this IEndpointRouteBuilder app) =>
        app.MapListTags()
            .MapRenameTag();
}
