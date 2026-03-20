using System.Net.Http.Headers;
using WeatherServer.Prompts;
using WeatherServer.Resources;
using WeatherServer.Tools;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("InspectorCors", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddMcpServer()
    .WithHttpTransport(options =>
    {
        options.Stateless = true;
    })
    .WithTools<WeatherTools>()
    .WithResources<WeatherAlertResources>()
    .WithPrompts<WeatherPrompts>();

builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});

builder.Services.AddHttpClient("WeatherApi", client =>
{
    client.BaseAddress = new Uri("https://api.weather.gov");
    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("weather-mcp-server", "1.0"));
});

builder.Services.AddHttpClient("Geocoding", client =>
{
    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("weather-mcp-server", "1.0"));
    client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US");
});

var app = builder.Build();
app.UseCors("InspectorCors");
app.MapMcp();
await app.RunAsync();