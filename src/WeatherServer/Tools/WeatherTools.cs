using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;

namespace WeatherServer.Tools;

[McpServerToolType]
public sealed class WeatherTools
{
    [McpServerTool, Description("Get the current weather forecast for a given city and state in the US.")]
    public static async Task<string> GetWeather(
        IHttpClientFactory httpClientFactory,
        [Description("The name of the city (e.g. San Francisco).")] string city,
        [Description("The US state (e.g. California or CA).")] string state)
    {
        // Step 1: Geocode the city and state to latitude/longitude
        var (latitude, longitude) = await GeocodeAsync(httpClientFactory, city, state);

        // Step 2: Get the forecast URL from the weather.gov points API
        using var client = httpClientFactory.CreateClient("WeatherApi");
        var pointUrl = string.Create(CultureInfo.InvariantCulture, $"/points/{latitude},{longitude}");
        using var pointResponse = await client.GetAsync(pointUrl);
        pointResponse.EnsureSuccessStatusCode();

        using var pointDocument = JsonDocument.Parse(await pointResponse.Content.ReadAsStringAsync());
        var forecastUrl = pointDocument.RootElement
            .GetProperty("properties")
            .GetProperty("forecast")
            .GetString()
            ?? throw new InvalidOperationException("No forecast URL returned from weather.gov.");

        // Step 3: Get the forecast data
        using var forecastResponse = await client.GetAsync(forecastUrl);
        forecastResponse.EnsureSuccessStatusCode();

        using var forecastDocument = JsonDocument.Parse(await forecastResponse.Content.ReadAsStringAsync());
        var periods = forecastDocument.RootElement
            .GetProperty("properties")
            .GetProperty("periods")
            .EnumerateArray();

        return string.Join("\n---\n", periods.Select(period => $"""
            {period.GetProperty("name").GetString()}
            Temperature: {period.GetProperty("temperature").GetInt32()}°F
            Wind: {period.GetProperty("windSpeed").GetString()} {period.GetProperty("windDirection").GetString()}
            Forecast: {period.GetProperty("detailedForecast").GetString()}
            """));
    }

    /// <summary>
    /// Geocodes a city and state to latitude and longitude with Census and a fallback provider.
    /// </summary>
    private static async Task<(double Latitude, double Longitude)> GeocodeAsync(IHttpClientFactory httpClientFactory, string city, string state)
    {
        using var geocodeClient = httpClientFactory.CreateClient("Geocoding");
        var nominatimUrl = $"https://nominatim.openstreetmap.org/search?city={Uri.EscapeDataString(city)}&state={Uri.EscapeDataString(state)}&country=USA&countrycodes=us&format=jsonv2&limit=1";
        using var nominatimResponse = await geocodeClient.GetAsync(nominatimUrl);
        nominatimResponse.EnsureSuccessStatusCode();

        using var nominatimDocument = JsonDocument.Parse(await nominatimResponse.Content.ReadAsStringAsync());
        if (nominatimDocument.RootElement.ValueKind == JsonValueKind.Array && nominatimDocument.RootElement.GetArrayLength() > 0)
        {
            var first = nominatimDocument.RootElement[0];
            var latitude = double.Parse(first.GetProperty("lat").GetString()!, CultureInfo.InvariantCulture);
            var longitude = double.Parse(first.GetProperty("lon").GetString()!, CultureInfo.InvariantCulture);

            return (Math.Round(latitude, 4), Math.Round(longitude, 4));
        }

        throw new InvalidOperationException($"Could not geocode '{city}, {state}'. Please check the city and state names.");
    }
}
