namespace DemoAspireApp.Web;

public class ApiClient(HttpClient httpClient)
{
    public async Task<WeatherForecast[]> GetWeatherAsync(int maxItems = 10, CancellationToken cancellationToken = default)
    {
        List<WeatherForecast>? forecasts = null;

        await foreach (var forecast in httpClient.GetFromJsonAsAsyncEnumerable<WeatherForecast>("/weatherforecast", cancellationToken))
        {
            if (forecasts?.Count >= maxItems)
            {
                break;
            }
            if (forecast is not null)
            {
                forecasts ??= [];
                forecasts.Add(forecast);
            }
        }

        return forecasts?.ToArray() ?? [];
    }

    public async Task<IReadOnlyCollection<Container>> GetTasksAsync(CancellationToken cancellationToken = default)
    {
        List<Container> containers = [];
        await foreach (var container in httpClient.GetFromJsonAsAsyncEnumerable<Container>("/containers", cancellationToken))
        {
            if (container is not null)
            {
                containers.Add(container);
            }
        }
        return containers;
    }
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public record Container(string Name);
