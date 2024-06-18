using Models;
using Newtonsoft.Json;
using System.Numerics;

namespace FlightsApi.Services
{
    public class FlightService
    {
        private readonly string _Aircraft = "https://localhost:0000/api/Aircraft/";
        private readonly string _Airport = "https://localhost:0000/api/Airport/";

        private async Task<bool> GetAirplaneAsync(string rab)
        {
            string url = _Aircraft + rab;
         
            try
            {
                using HttpClient client = new HttpClient();

                HttpResponseMessage response = await client.GetAsync(url);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> GetAirportAsync(string id)
        {
            string url = _Airport + id;

            try
            {
                using HttpClient client = new HttpClient();

                HttpResponseMessage response = await client.GetAsync(url);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ValidateAirplane(Aircraft plane) => await GetAirplaneAsync(plane.Rab);

        public async Task<bool> UpdateAirplane(Aircraft plane)
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

        public async Task<bool> ValidateAirport(Airport airport) => await GetAirportAsync(airport._id);
    }
}