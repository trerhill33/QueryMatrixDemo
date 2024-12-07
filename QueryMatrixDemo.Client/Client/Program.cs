using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using QueryMatrixDemo.Client.Client;
using QueryMatrixDemo.Core._old.Interfaces;
using QueryMatrixDemo.Core._old.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.Configure<StaticFileOptions>(options =>
{
    options.ServeUnknownFileTypes = true;
    options.DefaultContentType = "application/javascript";
});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IQueryMatrixService, QueryMatrixService>();
await builder.Build().RunAsync();
