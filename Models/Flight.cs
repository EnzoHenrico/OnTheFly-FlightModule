using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Flight
    {
        [Key]
        public int FlightNumber { get; set; }
        //[Key, Column(Order = 0)]
        public Airport Arrival { get; set; }
        //[Key, Column(Order = 1)]
        public Aircraft Plane { get; set; }
        //[Key, Column(Order = 3)]
        public DateTime Schedule { get; set; }
        public int Sales { get; set; }
        public bool Status { get; set; }
    }
}
