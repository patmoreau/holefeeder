using Carter;

using Microsoft.AspNetCore.Mvc;

namespace Holefeeder.Api.Features.Home;

[ApiExplorerSettings(IgnoreApi = true)]
public class Home : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("/", async (HttpContext context, IHostEnvironment environment) =>
        {
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.WriteAsync($"""
                                               <!DOCTYPE html>
                                               <html lang="en">

                                               <head>
                                                 <meta charset="utf-8">
                                                 <meta name="viewport" content="width=device-width, initial-scale=1">
                                                 <title>{environment.ApplicationName} - {environment.EnvironmentName}</title>
                                                 <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-T3c6CoIi6uLrA9TneNEoa7RxnatzjcDSCmG1MXxSR1GAsXEV/Dwwykc2MPK8M2HN" crossorigin="anonymous">
                                               </head>

                                               <body>
                                                 <div class="container my-5">
                                                   <h1>{environment.ApplicationName}</h1>
                                                   <p>Environment: {environment.EnvironmentName}</p>
                                                   <p><a href='/swagger'>Swagger</a></p>
                                                   <p><a href='/hangfire'>Hangfire Dashboard</a></p>
                                                   <p><a href='/hc-ui'>Health Checks</a></p>
                                                 </div>
                                               </body>
                                               </html>
                                               """);
        });
}
