using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;

namespace WeatherServer.Resources;

[McpServerResourceType]
public sealed class WeatherAlertResources
{
    [McpServerResource(
        UriTemplate = "weather://alerts/{state}",
        Name = "Weather Alerts by State",
        MimeType = "text/plain")]
    [Description("Get active weather alerts for a US state. Use the 2-letter state abbreviation (e.g. CA, NY, TX).")]
    public static async Task<string> GetAlertsByState(
        HttpClient client,
        [Description("The 2-letter US state abbreviation (e.g. CA, NY, TX).")] string state)
    {
        using var response = await client.GetAsync($"/alerts/active/area/{state}");
        response.EnsureSuccessStatusCode();

        using var jsonDocument = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var alerts = jsonDocument.RootElement
            .GetProperty("features")
            .EnumerateArray();

        if (!alerts.Any())
        {
            return $"No active weather alerts for {state}.";
        }

        return string.Join("\n--\n", alerts.Select(alert =>
        {
            var properties = alert.GetProperty("properties");
            return $"""
                Event: {properties.GetProperty("event").GetString()}
                Area: {properties.GetProperty("areaDesc").GetString()}
                Severity: {properties.GetProperty("severity").GetString()}
                Status: {properties.GetProperty("status").GetString()}
                Headline: {properties.GetProperty("headline").GetString()}
                Description: {properties.GetProperty("description").GetString()}
                Instruction: {properties.GetProperty("instruction").GetString()}
                """;
        }));
    }
}
