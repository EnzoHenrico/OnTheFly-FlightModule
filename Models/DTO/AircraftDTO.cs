using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Models.DTO
{
    public class AircraftDTO
    {
        [Key]
        [JsonProperty("rab")] public string Rab { get; set; }
        [JsonProperty("capacity")] public int Capacity { get; set; }
        [JsonProperty("dtRegistry")] public DateTime RegistryDate { get; set; }
        [JsonProperty("dtLastFlight")] public DateTime LastFlightDate { get; set; }
        [JsonProperty("cnpjCompany")] public string Company { get; set; }
    }
}