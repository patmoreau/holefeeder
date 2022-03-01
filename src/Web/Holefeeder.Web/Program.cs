using Holefeeder.Web.Config;
using Holefeeder.Web.Controllers;

using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services
  .AddControllersWithViews();
builder.Services.Configure<AngularSettings>(builder.Configuration.GetSection(nameof(AngularSettings)))
  .AddSingleton(sp => sp.GetRequiredService<IOptions<AngularSettings>>().Value);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
  name: "default",
  pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.AddConfigRoutes();

app.Run();
