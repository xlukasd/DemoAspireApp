using System.Net.Http.Json;

namespace DemoAspireApp.WeatherWorker
{
    public class WeatherApiClient(HttpClient httpClient)
    {
        public async Task<WeatherForecast[]> GetWeatherAsync(CancellationToken cancellationToken = default)
        {
            List<WeatherForecast>? forecasts = null;

            await foreach (var forecast in httpClient.GetFromJsonAsAsyncEnumerable<WeatherForecast>("/weatherforecast", cancellationToken))
            {
                if (forecast is not null)
                {
                    forecasts ??= [];
                    forecasts.Add(forecast);
                }
            }

            return forecasts?.ToArray() ?? [];
        }
    }

    public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary);
}