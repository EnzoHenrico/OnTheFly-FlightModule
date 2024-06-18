using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Aircraft
    {
        [Key]
        [JsonProperty("rab")] public string Rab { get; set; }
        [JsonProperty("capacity")] public int Capacity { get; set; }
        [JsonProperty("registry_date")] public DateTime RegistryDate { get; set; }
        [JsonProperty("last_flight_date")] public DateTime LastFlightDate { get; set; }
        [JsonProperty("company")] public Company Company { get; set; }
    }
}
