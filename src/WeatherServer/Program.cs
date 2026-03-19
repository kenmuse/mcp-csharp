using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using WeatherServer.Prompts;
using WeatherServer.Resources;
using WeatherServer.Tools;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<WeatherTools>()
    .WithResources<WeatherAlertResources>()
    .WithPrompts<WeatherPrompts>();

builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});

using var httpClient = new HttpClient { BaseAddress = new Uri("https://api.weather.gov") };
httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("weather-mcp-server", "1.0"));
builder.Services.AddSingleton(httpClient);

await builder.Build().RunAsync();
