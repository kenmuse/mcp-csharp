# MCP Weather Server

A C# (.NET 9) MCP (Model Context Protocol) server that provides weather information using the [National Weather Service API](https://api.weather.gov). Built with the official [MCP C# SDK](https://github.com/modelcontextprotocol/csharp-sdk).

## Features

### 🔧 Tool: GetWeather
Get the current weather forecast for any US city and state.
- Automatically geocodes city/state to coordinates using the US Census Bureau API
- Returns detailed forecast periods including temperature, wind, and conditions

### 📋 Resource: Weather Alerts by State
Access active weather alerts for any US state via the resource URI pattern:
```
weather://alerts/{state}
```
Where `{state}` is a 2-letter state abbreviation (e.g., `CA`, `NY`, `TX`).

### 💬 Prompt: Weather Briefing
A sample prompt template that generates a comprehensive weather briefing for a given city and state. It instructs the LLM to:
1. Fetch the current forecast using the GetWeather tool
2. Check for active alerts using the weather alerts resource
3. Provide a summary with recommendations

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 22+](https://nodejs.org/) (for the MCP Inspector)

## Getting Started

### Build and Run

```bash
cd src/WeatherServer
dotnet build
dotnet run
```

The server communicates over stdio using the MCP protocol.

### Using the MCP Inspector

The [MCP Inspector](https://modelcontextprotocol.io/docs/tools/inspector) provides a web UI for interacting with the MCP server.

```bash
npx @modelcontextprotocol/inspector dotnet run --project src/WeatherServer/WeatherServer.csproj
```

This will start:
- **Inspector UI** at http://localhost:6274
- **Proxy server** on port 6277

From the Inspector, you can:
- Browse and invoke the `GetWeather` tool
- Access weather alert resources by state
- Use the `weather_briefing` prompt template

## Dev Container

This project includes a dev container configuration for VS Code / GitHub Codespaces.

### Features
- .NET 9 SDK pre-installed
- Node.js 22 for running the MCP Inspector
- VS Code extensions for C# development
- Pre-configured build and debug tasks
- Port forwarding for MCP Inspector (ports 6274 and 6277)

### VS Code Tasks
- **build** (Ctrl+Shift+B): Build the project
- **inspect**: Launch the MCP Inspector connected to the weather server

### Debugging
Use the **Launch Weather Server** configuration in VS Code to debug the server with breakpoints.

## Project Structure

```
├── .devcontainer/
│   └── devcontainer.json          # Dev container configuration
├── .vscode/
│   ├── extensions.json            # Recommended extensions
│   ├── launch.json                # Debug configuration
│   └── tasks.json                 # Build and inspect tasks
└── src/WeatherServer/
    ├── Program.cs                 # Application entry point
    ├── Tools/
    │   └── WeatherTools.cs        # GetWeather tool implementation
    ├── Resources/
    │   └── WeatherAlertResources.cs # Weather alerts resource
    ├── Prompts/
    │   └── WeatherPrompts.cs      # Sample prompt template
    └── WeatherServer.csproj       # Project file
```

## API Data Sources

- **Weather Forecasts**: [NWS Points API](https://api.weather.gov/points/{lat},{lon})
- **Weather Alerts**: [NWS Alerts API](https://api.weather.gov/alerts/active/area/{state})
- **Geocoding**: [US Census Bureau Geocoder](https://geocoding.geo.census.gov/geocoder/)