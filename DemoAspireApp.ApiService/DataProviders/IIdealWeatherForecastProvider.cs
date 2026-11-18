namespace DemoAspireApp.ApiService.DataProviders
{
    internal interface IIdealWeatherForecastProvider
    {
        Task<WeatherForecast> GetIdealWeatherForecastAsync();
    }
}
