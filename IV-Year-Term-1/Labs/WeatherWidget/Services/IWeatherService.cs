using System;
using System.Threading.Tasks;
using WeatherWidget.Models;

namespace WeatherWidget.Services
{
    public interface IWeatherService
    {
        Task<Weather> GetWeather(DateTime date);
    }
}