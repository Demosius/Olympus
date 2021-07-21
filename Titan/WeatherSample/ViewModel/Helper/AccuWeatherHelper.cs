using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherApp.Model;

namespace WeatherApp.ViewModel.Helper
{
    public class AccuWeatherHelper
    {
        public const string BASE_URL = "http://dataservice.accuweather.com/";
        public const string AUTOCOMPLETE_ENDPOINT = "locations/v1/cities/autocomplete?apikey={0}&q={1}";
        public const string CURRENT_CONDITIONS_ENDPOINT = "currentconditions/v1/{0}?apikey={1}";
        public const string API_KEY = "xkzjpmj6nI1Abme8bE3sBjkstCMEEb31";

        public static async Task<List<City>> GetCities(string query)
        {
            List<City> cities = new List<City>();

            string url = BASE_URL + string.Format(AUTOCOMPLETE_ENDPOINT, API_KEY, query);

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();

                cities = JsonSerializer.Deserialize<List<City>>(json);
            }

            return cities;
        }

        public static async Task<CurrentConditions> GetCurrentConditions(string cityKey)
        {
            CurrentConditions currentConditions = new CurrentConditions();

            string url = BASE_URL + string.Format(CURRENT_CONDITIONS_ENDPOINT, cityKey, API_KEY);

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();

                currentConditions = JsonSerializer.Deserialize<List<CurrentConditions>>(json).FirstOrDefault();
            }

            return currentConditions;
        }

    }
}
