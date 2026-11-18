namespace DemoAspireApp.ApiService.DataProviders
{
    internal class IdealWeatherForecastProvider : IIdealWeatherForecastProvider
    {
        public Task<WeatherForecast> GetIdealWeatherForecastAsync()
        {
            var idealForecast = new WeatherForecast(DateOnly.FromDateTime(DateTime.Now), 25, "Mild");
            return Task.FromResult(idealForecast);
        }
    }
}
