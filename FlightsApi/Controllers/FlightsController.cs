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

            List<Flight> flights = await _context.Flight.Include(a => a.Arrival).Include(a => a.Plane).ToListAsync();
            List<FlightDTO> flightsDTO = new List<FlightDTO>();

            foreach (var flight in flights)
            {
                var flightDTO = new FlightDTO();
                flightDTO.FlightNumber = flight.FlightNumber;
                flightDTO.ArrivalIata = flight.Arrival.Iata;
                flightDTO.PlaneRab = flight.Plane.Rab;
                flightDTO.Sales = flight.Sales ;
                flightDTO.Status = flight.Status;
                flightDTO.Schedule = flight.Schedule;

                flightsDTO.Add(flightDTO);
            }
            
            return flightsDTO;
        }

        // GET: Flights/5
        [HttpGet("{flightNumber}")]

        public async Task<ActionResult<FlightDTO>> GetOne(int? flightNumber)
        {
            if (_context.Flight == null)
            {
                return Problem("Entity set 'FlightsApiContext.Flights' is null.");
            }

            if (flightNumber == null)
            {
                return NotFound();
            }

            Flight? flight = await _context.Flight.Include(a => a.Arrival).Include(a => a.Plane).FirstOrDefaultAsync(flight => flight.FlightNumber == flightNumber);

            var flightDTO = new FlightDTO();
            flightDTO.FlightNumber = flight.FlightNumber;
            flightDTO.ArrivalIata = flight.Arrival.Iata;
            flightDTO.PlaneRab = flight.Plane.Rab;
            flightDTO.Sales = flight.Sales;
            flightDTO.Status = flight.Status;
            flightDTO.Schedule = flight.Schedule;        


            if (flight == null)
            {
                return NotFound();
            }
            return flightDTO;
        }

        // Função de retornar apenas voos ativos, implementação secundária!!
        // GET: Flights/Actives

        [HttpGet("Actives")]
        public async Task<ActionResult<IEnumerable<FlightDTO>>> GetAllActives()
        {
            if (_context.Flight == null)
            {
                return Problem("Entity set 'FlightsApiContext.Flights' is null.");
            }

            List<Flight> flights = await _context.Flight.Include(a => a.Arrival).Include(a => a.Plane).Where(flight => flight.Status == true).ToListAsync();
            List<FlightDTO> flightsDTO = new List<FlightDTO>();

            foreach (var flight in flights)
            {
                var flightDTO = new FlightDTO();
                flightDTO.FlightNumber = flight.FlightNumber;
                flightDTO.ArrivalIata = flight.Arrival.Iata;
                flightDTO.PlaneRab = flight.Plane.Rab;
                flightDTO.Sales = flight.Sales;
                flightDTO.Status = flight.Status;
                flightDTO.Schedule = flight.Schedule;

                flightsDTO.Add(flightDTO);
            }

            if (flights == null)
            {
                return NotFound();
            }
            return flightsDTO;
        }

        // POST: Flights/Add
        [HttpPost("Add")]
        public async Task<ActionResult<Flight>> Create(FlightDTO dto)
        {
            if (_context.Flight == null || dto == null)
                return Problem("Null context or input.");

            if (!await _flightService.ValidateAirplaneAsync(dto.PlaneRab))
                return Problem("Invalid aircraft.");

            else if (!await _flightService.ValidateAirportAsync(dto.ArrivalIata))
                return Problem("Invalid arrival.");

            Flight flight = new Flight
            {
                FlightNumber = dto.FlightNumber,
                Arrival = await _flightService.GetAirportAsync(dto.ArrivalIata),
                Plane = await _flightService.GetAirplaneAsync(dto.PlaneRab),
                Sales = dto.Sales,
                Status = dto.Status,
                Schedule = dto.Schedule
            };

            flight.Plane.LastFlightDate = flight.Schedule;
            flight.Plane.Capacity -= flight.Sales;

            if (!await _flightService.UpdateAirplaneAsync(flight.Plane))
                return Problem("Failed to update aircraft last flight.");

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
            bool _opCheck;
            if(operation != "Cancel" && operation != "Sell")
            {
                _opCheck = false;
            }
            else
            {
                _opCheck = true;
            }
            if (id == null || newSeats < 0 || _opCheck != true)
            {
                return NotFound("Unavaliable Id, Operation or Seat Numbers");
            }

            var flight = await _context.Flight.Where(p => p.FlightNumber == id)
                .Include(a => a.Plane).Include(c => c.Plane.Company)
                .Include(c => c.Plane.Company.Address).Include(b => b.Arrival).FirstOrDefaultAsync();
           
            if (flight == null)
            {
                return NotFound("The flight doesn't exists");
            }

            bool? _notBlocked = flight.Plane.Company.Status;

            if (_notBlocked == false)
            {
                return BadRequest($"Blockd Company Transition, try to contact {flight.Plane.Company.Name}");
            }

            if (flight.Status == false)
            {
                return BadRequest($"The Flight isn't avaliable for some reason, try /ChangeStatus/{id}");
            }

            // Checking the avaliable seats
            int _num = 0;
            if(operation == "Cancel")
            {
                _num = -(newSeats ?? 0);
            }else if(operation == "Sell")
            {
                _num = (newSeats ?? 0);
            }
            int _maxCapacity = flight.Plane.Capacity;
            int _currentCapacity = flight.Sales;
            int _newCapacity = _currentCapacity + _num;
            int _avaliableSeats = _maxCapacity - _newCapacity;

            if (_currentCapacity == _maxCapacity && operation != "Cancel")
            {
                // For automatic changes of status when check out of capacity
                //if(flight.Status = true)
                //{
                //    flight.Status = !flight.Status;
                //}
                return BadRequest("There isn't an avaliable passage to buy");
            }
            if (_newCapacity <= _maxCapacity)
            {
                flight.Sales = _newCapacity;
                // For automatic changes of status when fullfill the capacity
                //if (_newCapacity == _maxCapacity)
                //{
                //    flight.Status = !flight.Status;
                //}
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
            return Ok($"{operation} on id {id} concluded for {newSeats} passages\nThere is {_avaliableSeats} avaliable seats to sell");
        }

        private bool FlightExists(int id)
        {
          return (_context.Flight?.Any(e => e.FlightNumber == id)).GetValueOrDefault();
        }
    }
}
