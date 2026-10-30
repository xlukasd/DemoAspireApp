namespace DemoAspireApp.ApiService
{
    internal interface IIdealWeatherForecastProvider
    {
        Task<WeatherForecast> GetIdealWeatherForecastAsync();
    }
}
