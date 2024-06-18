using Models;
using Newtonsoft.Json;
using System.Numerics;

namespace FlightsApi.Services
{
    public class FlightService
    {
        private readonly string _Aircraft = "https://localhost:0000/api/Aircraft/";
        private readonly string _Airport = "https://localhost:0000/api/Airport/";

        private async Task<Aircraft?> GetAirplaneAsync(string rab)
        {
            Aircraft? aircraft = null;
            string url = _Aircraft + rab;
            
            try
            {
                using HttpClient client = new HttpClient();

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    aircraft = JsonConvert.DeserializeObject<Aircraft>(json);
                }
            }
            catch (Exception) { }

            return aircraft;
        }

        private async Task<Airport?> GetAirportAsync(string id)
        {
            Airport? airport = null;
            string url = _Airport + id;

            try
            {
                using HttpClient client = new HttpClient();

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    airport = JsonConvert.DeserializeObject<Airport>(json);
                }
            }
            catch (Exception) { }

            return airport;
        }

        public async Task<bool> ValidateAirplaneAsync(string rab) => await GetAirplaneAsync(rab) != null;

        public async Task<bool> UpdateAirplaneLastFlightAsync(Aircraft plane)
        {
            string url = _Aircraft + plane.Rab;

            try
            {
                using HttpClient client = new HttpClient();

                HttpResponseMessage response = await client.PutAsync(url, new StringContent(JsonConvert.SerializeObject(plane)));

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ValidateAirportAsync(string airportId) => await GetAirportAsync(airportId) != null;
    }
}