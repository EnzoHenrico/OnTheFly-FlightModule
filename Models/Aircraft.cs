﻿using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Aircraft
    {
        [Key]
        [JsonProperty("rab")] public string Rab { get; set; }
        [JsonProperty("capacity")] public int Capacity { get; set; }
        [JsonProperty("dtRegistry")] public DateTime RegistryDate { get; set; }
        [JsonProperty("dtLastFlight")] public DateTime LastFlightDate { get; set; }
        [JsonProperty("cnpjCompany")] public Company Company { get; set; }
    }

}