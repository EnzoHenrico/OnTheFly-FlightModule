using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace FlightsApi.Data
{
    public class FlightsApiContext : DbContext
    {
        public FlightsApiContext (DbContextOptions<FlightsApiContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>()
                .HasKey(address => new { address.ZipCode, address.Number });

            modelBuilder.Entity<Company>()
                .HasKey(company => new { company.Cnpj, company.Name });

            //modelBuilder.Entity<Flight>()
            //    .HasKey(flight => new { flight.Arrival, flight.Plane, flight.Schedule });

            modelBuilder.Entity<Airport>()
                .HasKey(airport => new { airport._id });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Models.Flight> Flight { get; set; }
        public DbSet<Models.Airport> Airport { get; set; }
        public DbSet<Models.Aircraft> Aircraft { get; set; }
        public DbSet<Models.Company> Company { get; set; }
        public DbSet<Models.Address> Address { get; set; }
    }
}
