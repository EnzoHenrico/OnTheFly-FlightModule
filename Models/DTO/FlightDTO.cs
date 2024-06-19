using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class FlightDTO
    {
        [JsonProperty("flightNumber")] public int FlightNumber { get; set; }
        [JsonProperty("arrivalIata")] public string ArrivalIata { get; set; }
        [JsonProperty("planeRab")] public string PlaneRab { get; set; }
        [JsonProperty("sales")] public int Sales { get; set; }
        [JsonProperty("status")] public bool Status { get; set; }
        [JsonProperty("schedule")] public DateTime Schedule { get; set; }
    }
}
