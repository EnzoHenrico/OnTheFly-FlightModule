using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightsApi.Data;
using FlightsApi.Services;
using Models;
using NuGet.Protocol.Core.Types;

namespace FlightsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController : Controller
    {
        private readonly FlightsApiContext _context;
        private readonly FlightService _flightService;

        public FlightsController(FlightsApiContext context, FlightService flightService)
        {
            _context = context;
            _flightService = flightService;
        }

        // GET: Flights
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flight>>> GetAll()
        {
            if (_context.Flight == null)
            {
                return Problem("Entity set 'FlightsApiContext.Flights' is null.");
            }

            var flights = await _context.Flight.ToListAsync();

            return flights;
        }

        // GET: Flights/5
        [HttpGet("{flightNumber}")]
        public async Task<ActionResult<Flight>> GetOne(int? flighNumber)
        {
            if (_context.Flight == null)
            {
                return Problem("Entity set 'FlightsApiContext.Flights' is null.");
            }

            if (flighNumber == null)
            {
                return NotFound();
            }

            var flight = await _context.Flight.FirstOrDefaultAsync(flight => flight.FlightNumber == flighNumber);
            if (flight == null)
            {
                return NotFound();
            }

            return flight;
        }

        // Função de retornar apenas voos ativos, implementação secundária!!
        // GET: Flights/Actives
        [HttpGet("{flightNumber}")]
        public async Task<ActionResult<IEnumerable<Flight>>> GetAllActives()
        {
            if (_context.Flight == null)
            {
                return Problem("Entity set 'FlightsApiContext.Flights' is null.");
            }

            var flights = await _context.Flight.Where(flight => flight.Status == true).ToListAsync();
            if (flights == null)
            {
                return NotFound();
            }

            return flights;
        }

        // POST: Flights/Add
        [HttpPost("Add")]
        public async Task<ActionResult<Flight>> Create(Flight flight)
        {
            if (_context.Flight == null)
            {
                return Problem("Entity set 'FlightsApiContext.Flights' is null.");
            }

            if (!await _flightService.ValidateAirplaneAsync(flight.Plane))
                return Problem("Invalid aircraft.");
            
            else if (!await _flightService.ValidateAirportAsync(flight.Arrival))
                return Problem("Invalid arrival.");

            else if (!await _flightService.UpdateAirplaneLastFlightAsync(flight.Plane))
                return Problem("Failed to update aircraft last flight.");

            _context.Flight.Add(flight);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOne", new { flightNumber = flight.FlightNumber }, flight);
        }

        // PATCH: Flights/ChangeStatus/5
        [HttpPatch("Activate/{id}/{status}")]
        public async Task<ActionResult> PatchStatus(int? id, bool status)
        {
            if (_context.Flight == null)
            {
                return Problem("Entity set 'FlightsApiContext.Flights' is null.");
            }

            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flight.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }

            flight.Status = status;

            return NoContent();
        }

        private bool FlightExists(int id)
        {
          return (_context.Flight?.Any(e => e.FlightNumber == id)).GetValueOrDefault();
        }
    }
}
