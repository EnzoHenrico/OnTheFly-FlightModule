using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Company
    {
        [Key, Column(Order = 0)]
        [JsonProperty("cnpj")] public string Cnpj { get; set; }
        [Key, Column(Order = 1)]
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("name_opt")] public string NameOpt { get; set; }
        [JsonProperty("opening_date")] public DateTime OpeningDate { get; set; }
        [JsonProperty("status")] public bool? Status { get; set; }
        [JsonProperty("address")] public Address Address { get; set; }
    }
}
