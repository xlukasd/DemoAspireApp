using Microsoft.Extensions.AI;

namespace DemoAspireApp.WeatherWorker;

public class Worker(WeatherApiClient weatherApiClient, IChatClient chatClient, ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            try
            {
                WeatherForecast[] weather = await weatherApiClient.GetWeatherAsync(stoppingToken);

                ChatMessage systemMessage = new(ChatRole.System, "You are a helpful assistant that provides information based on weather forecast.");

                var userMessage = new ChatMessage(
                    ChatRole.User, 
                    $"I would like to know, what should I do next 5 days based on following weather forecast for next 5 days: {string.Join(" -- ", weather.Select(w => $"{w.Summary} {w.TemperatureC}°C"))}");

                ChatResponse chatResponse = await chatClient.GetResponseAsync([systemMessage, userMessage], cancellationToken: stoppingToken);

                ChatMessage? responseMessage = chatResponse.Messages.SingleOrDefault(x => x.Role == ChatRole.Assistant);

                if (responseMessage is not null)
                {
                    Console.WriteLine();
                    Console.WriteLine("------------ ACTIVITY SUGGESTION ------------");
                    Console.WriteLine(responseMessage.Text);
                    Console.WriteLine("---------------------------------------------");
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("No assistant response received from chat client.");
                    logger.LogWarning("No assistant response received from chat client.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing weather data.");
            }

            await Task.Delay(10000, stoppingToken);
        }
    }
}
