 using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Models
{
    public class Address
    {    
        [Key, Column(Order = 0)]
        [JsonProperty("zip")] public string ZipCode { get; set; }
        [Key, Column(Order = 1)]
        [JsonProperty("number")] public int Number { get; set; }
        [JsonProperty("street")] public string? Street { get; set; }
        [JsonProperty("complement")] public string Complement { get; set; }
        [JsonProperty("city")] public string City { get; set; }
        [JsonProperty("state")] public string State { get; set; }
    }
}
