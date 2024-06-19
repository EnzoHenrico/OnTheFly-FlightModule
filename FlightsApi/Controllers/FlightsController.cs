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
using Models.DTO;

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
        public async Task<ActionResult<IEnumerable<FlightDTO>>> GetAll()
        {
            if (_context.Flight == null)
            {
                return Problem("Entity set 'FlightsApiContext.Flights' is null.");
            }

            var flights = await _context.Flight.ToListAsync();

            List<FlightDTO> result = new List<FlightDTO>();
            foreach(Flight flight in flights)
            {
                result.Add(new FlightDTO(flight));
            }

            return result;
        }

        // GET: Flights/5
        [HttpGet("{flightNumber}")]
        public async Task<ActionResult<FlightDTO>> GetOne(int? flighNumber)
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

            return new FlightDTO(flight);
        }

        // Função de retornar apenas voos ativos, implementação secundária!!
        // GET: Flights/Actives
        [HttpGet("{flightNumber}")]
        public async Task<ActionResult<IEnumerable<FlightDTO>>> GetAllActives()
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

            List<FlightDTO> result = new List<FlightDTO>();
            foreach (Flight flight in flights)
            {
                result.Add(new FlightDTO(flight));
            }

            return result;
        }

        // POST: Flights/Add
        [HttpPost("Add")]
        public async Task<ActionResult<Flight>> Create(Flight flight)
        {
            if (_context.Flight == null)
            {
                return Problem("Entity set 'FlightsApiContext.Flights' is null.");
            }

            _context.Flight.Add(flight);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOne", new { flightNumber = flight.FlightNumber }, flight);
        }

        // PATCH: /ChangeStatus/1
        [HttpPatch("ChangeStatus/{id}")]
        public async Task<ActionResult> PatchStatus(int? id)
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

            flight.Status = !flight.Status;

            try
            {
                _context.Flight.Update(flight);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightExists(flight.FlightNumber))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PATCH: /CheckSeats/1/Sell/1
        // Cancel is for decrementation and Sell is for sold seats
        [HttpPatch("CheckSeats/{id}/{operation}/{newSeats}")]
        public async Task<ActionResult> PatchSeats(int? id, string? operation, int? newSeats)
        {
            if (_context.Flight == null)
            {
                return Problem("Entity set 'FlightsApiContext.Flights' is null.");
            }

            if (id == null)
            {
                return NotFound("Unavaliable Id");
            }

            var flight = await _context.Flight.Where(p => p.FlightNumber == id).Include(a => a.Plane).Include(c => c.Plane.Company).Include(c => c.Plane.Company.Address).Include(b => b.Arrival).FirstOrDefaultAsync();


            /*
             
                var blog1 = context.Blogs
                       .Where(b => b.Name == "ADO.NET Blog")
                       .Include(b => b.Posts)
                       .FirstOrDefault();
            */

            if (flight == null)
            {
                return NotFound("The flight doesn't exists");
            }

            // Checking the avaliable seats
            int _num;
            if(operation == "Cancel")
            {
                _num = -(newSeats ?? 0);
            }else if(operation == "Sell")
            {
                _num = (newSeats ?? 0);
            }
            else
            {
                return NotFound("Invalid Operation, try these options:\n\tCancel\n\tSell");
            }
            bool? _notBlocked = flight.Plane.Company.Status;
            if (_notBlocked == false)
            {
                return NotFound($"Blockd Company Transition, try to contact {flight.Plane.Company.Name}");
            }
            //if(flight.Status == false)
            //{
            //    return BadRequest("All seats are already sold up");
            //}
            int _maxCapacity = flight.Plane.Capacity;
            int _currentCapacity = flight.Sales;
            int _newCapacity = _currentCapacity + _num;

            if (_newCapacity <= _maxCapacity)
            {
                flight.Sales = _newCapacity;
                if(_newCapacity == _maxCapacity)
                {
                    flight.Status = !flight.Status;
                }
            }
            else
            {
                return BadRequest($"There isn't avaliable flights to the selected destiny");
            }

            // Updating changes
            try
            {
                _context.Flight.Update(flight);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightExists(flight.FlightNumber))
                {
                    return NotFound("The flight doesn't existis");
                }
                else
                {
                    throw;
                }
            }

            return Ok($"{operation} on id {id} concluded");
        }

        private bool FlightExists(int id)
        {
          return (_context.Flight?.Any(e => e.FlightNumber == id)).GetValueOrDefault();
        }
    }
}
