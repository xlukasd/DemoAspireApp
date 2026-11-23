using DemoAspireApp.WeatherWorker;

var builder = Host.CreateApplicationBuilder(args);

builder
    .AddOllamaApiClient("ollama-phi3")
    .AddChatClient();

builder.Services.AddHttpClient<WeatherApiClient>(client =>
{
    client.BaseAddress = new("https+http://apiservice");
});

builder.AddServiceDefaults(enableStandardResilienceHandler: false);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
