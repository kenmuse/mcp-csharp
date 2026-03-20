using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace WeatherServer.Prompts;

[McpServerPromptType]
public sealed class WeatherPrompts
{
    [McpServerPrompt(Name = "weather_briefing"), Description("Generate a weather briefing for a city and state, including current forecast and any active alerts.")]
    public static IEnumerable<ChatMessage> WeatherBriefing(
        [Description("The city name (e.g. San Francisco)")] string city,
        [Description("The US state (e.g. California or CA)")] USState state)
    {
        var stateAbbreviation = state.ToString();
        return
        [
            new ChatMessage(ChatRole.User, $"""
                Please provide a comprehensive weather briefing for {city}, {state}. Include:

                1. First, use the GetWeather tool to get the current forecast for {city}, {state}.
                2. Then, use the weather://alerts/{stateAbbreviation} resource to check for any active weather alerts in {state}.
                3. Summarize the information in a clear, concise briefing format that includes:
                   - Current conditions and temperature
                   - Extended forecast highlights
                   - Any active weather alerts or warnings
                   - Recommendations for the day (e.g. bring an umbrella, dress warmly)

                Please format the response in a readable way.
                """)
        ];
    }
}
