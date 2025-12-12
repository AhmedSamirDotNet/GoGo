using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Snap.APIs.DTOs;
using Snap.Core.Entities;
using Snap.Repository.Data;
using Snap.APIs.Errors;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Snap.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TripsHistoryController : ControllerBase
    {
        private readonly SnapDbContext _context;

        public TripsHistoryController(SnapDbContext context)
        {
            _context = context;
        }

        // POST: api/TripsHistory
        [HttpPost]
        public async Task<IActionResult> CreateTripsHistory([FromBody] CreateTripsHistoryDto dto)
        {
            var driver = await _context.Drivers.FindAsync(dto.DriverId);
            if (driver == null)
                return NotFound(new ApiResponse(404, "Driver not found"));

            var trip = new TripsHistory
            {
                Review = dto.Review,
                PaymentWay = dto.PaymentWay,
                From = dto.From,
                To = dto.To,
                Date = dto.Date,
                TotalTip = dto.TotalTip,
                DriverId = dto.DriverId
            };

            _context.TripsHistories.Add(trip);
            await _context.SaveChangesAsync();

            var result = new TripsHistoryDto
            {
                Id = trip.Id,
                Review = trip.Review,
                PaymentWay = trip.PaymentWay,
                From = trip.From,
                To = trip.To,
                Date = trip.Date,
                TotalTip = trip.TotalTip,
                DriverId = trip.DriverId
            };

            return Ok(result);
        }

        // GET: api/TripsHistory
        [HttpGet]
        public async Task<ActionResult<List<TripsHistoryDto>>> GetAllTripsHistories()
        {
            var trips = await _context.TripsHistories
                .Select(t => new TripsHistoryDto
                {
                    Id = t.Id,
                    Review = t.Review,
                    PaymentWay = t.PaymentWay,
                    From = t.From,
                    To = t.To,
                    Date = t.Date,
                    TotalTip = t.TotalTip,
                    DriverId = t.DriverId
                })
                .ToListAsync();

            return Ok(trips);
        }

        // GET: api/TripsHistory/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TripsHistoryDto>> GetTripsHistoryById(int id)
        {
            var trip = await _context.TripsHistories.FindAsync(id);
            if (trip == null)
                return NotFound(new ApiResponse(404, "Trip history not found"));

            var dto = new TripsHistoryDto
            {
                Id = trip.Id,
                Review = trip.Review,
                PaymentWay = trip.PaymentWay,
                From = trip.From,
                To = trip.To,
                Date = trip.Date,
                TotalTip = trip.TotalTip,
                DriverId = trip.DriverId
            };

            return Ok(dto);
        }

        // GET: api/TripsHistory/user/{userId}
        [HttpGet("driver/{userId}")]
        public async Task<ActionResult<List<TripsHistoryDto>>> GetTripsHistoriesByUserId(string userId)
        {
            // Find the driver with the given userId
            var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.UserId == userId);
            if (driver == null)
                return NotFound(new ApiResponse(404, "Driver not found"));

            var trips = await _context.TripsHistories
                .Where(t => t.DriverId == driver.Id)
                .Select(t => new TripsHistoryDto
                {
                    Id = t.Id,
                    Review = t.Review,
                    PaymentWay = t.PaymentWay,
                    From = t.From,
                    To = t.To,
                    Date = t.Date,
                    TotalTip = t.TotalTip,
                    DriverId = t.DriverId
                })
                .ToListAsync();

            return Ok(trips);
        }
    }
}