using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WeatherWidget.Models;

namespace WeatherWidget.Services
{
    public class WeatherService : IWeatherService
    {
        public async Task<Weather> GetWeather(DateTime date)
        {
            var queryString = "https://api.worldweatheronline.com/premium/v1/weather.ashx?key=5f1805ca27f64b978bc155938172811&q=Kharkiv&format=json&num_of_days=1";

            var result = await GetDataFromService(queryString);
            var weatherCurrentCondition = ((JArray)result["data"]["current_condition"]).First;

            return new Weather()
            {
                Title = ((JArray)weatherCurrentCondition["weatherDesc"]).First["value"].ToString(),
                Temperature = weatherCurrentCondition["temp_C"].ToString(),
                Humidity = weatherCurrentCondition["humidity"].ToString(),
                Visibility = weatherCurrentCondition["visibility"].ToString(),
                Wind = weatherCurrentCondition["windspeedKmph"].ToString() + " km/h"
            };
        }

        private static async Task<dynamic> GetDataFromService(string queryString)
        {
            var request = (HttpWebRequest)WebRequest.Create(queryString);

            var response = await request.GetResponseAsync().ConfigureAwait(false);
            var stream = response.GetResponseStream();

            var streamReader = new StreamReader(stream);
            string responseText = streamReader.ReadToEnd();

            dynamic data = JsonConvert.DeserializeObject(responseText);

            return data;
        }
    }
}