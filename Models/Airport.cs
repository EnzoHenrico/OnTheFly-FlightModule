using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Airport
    {
        [Key]
        [JsonProperty("_id")] public string _id { get; set; }
        [JsonProperty("iata")] public string Iata { get; set; }
        [JsonProperty("state")] public string State { get; set; }
        [JsonProperty("city")] public string City { get; set; }
        [JsonProperty("country")] public string Country { get; set; }
    }
}
