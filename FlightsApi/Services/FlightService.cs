using Models;
using Models.DTO;
using Newtonsoft.Json;
using System.Text;

namespace FlightsApi.Services
{
    public class FlightService
    {
        private readonly string _Aircraft = "https://localhost:7051/api/AirCrafts/";
        private readonly string _Airport = "https://localhost:44366/Airport/";
        private readonly string _Company = "https://localhost:7269/api/Companies/";

        public async Task<Company?> GetCompanyAsync(string cnpj)
        {
            Company? company = null;
            string url = _Company + cnpj;

            try
            {
                using HttpClient client = new HttpClient();

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    company = JsonConvert.DeserializeObject<Company>(json);
                }
            }
            catch (Exception e) { Console.WriteLine(e); }

            return company;
        }

        public async Task<Aircraft?> GetAirplaneAsync(string rab)
        {
            Aircraft? plane = null;
            string url = _Aircraft + rab;

            try
            {
                using HttpClient client = new HttpClient();

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    if (json == null)
                        return plane;

                    AircraftDTO? dto = JsonConvert.DeserializeObject<AircraftDTO>(json);

                    if (dto != null)
                    {
                        var company = GetCompanyAsync(dto.Company).Result;

                        if (company == null)
                        {
                            company = new Company { Cnpj = dto.Company, Name = "Not Founded", NameOpt = " ", OpeningDate = DateTime.Now, Status = true };

                            company.Address = new Address { City = " ", State = " ", Street = " ", ZipCode = " ", Complement = " ", Number = 1 };
                        }

                        plane = new Aircraft
                        {
                            Rab = dto.Rab,
                            Capacity = dto.Capacity,
                            RegistryDate = dto.RegistryDate,
                            LastFlightDate = dto.LastFlightDate,
                            Company = company
                        };
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return plane;
        }

        public async Task<Airport?> GetAirportAsync(string id)
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

            airport.State = " ";

            return airport;
        }

        public async Task<bool> ValidateAirplaneAsync(string rab) => await GetAirplaneAsync(rab) != null;

        public async Task<bool> UpdateAirplaneAsync(Aircraft plane)
        {
            string url = _Aircraft;

            AircraftDTO aircraftDTO = new AircraftDTO
            {
                Rab = plane.Rab,
                Capacity = plane.Capacity,
                RegistryDate = plane.RegistryDate,
                LastFlightDate = plane.LastFlightDate,
                Company = plane.Company.Cnpj
            };

            try
            {
                using HttpClient client = new HttpClient();

                HttpResponseMessage response = await client.PutAsync(url, new StringContent(JsonConvert.SerializeObject(aircraftDTO), Encoding.UTF8, "application/json"));

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