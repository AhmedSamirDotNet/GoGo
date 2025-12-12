using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Snap.APIs.DTOs;
using Snap.Core.Entities;
using Snap.Repository.Data;
using Snap.APIs.Errors;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Snap.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserHistoryController : ControllerBase
    {
        private readonly SnapDbContext _context;

        public UserHistoryController(SnapDbContext context)
        {
            _context = context;
        }

        // POST: api/UserHistory
        [HttpPost]
        public async Task<IActionResult> CreateUserHistory([FromBody] CreateUserHistoryDto dto)
        {
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
                return NotFound(new ApiResponse(404, "User not found"));


            var history = new UserHistory
            {
                UserId = dto.UserId,
                From = dto.From,
                To = dto.To,
                Price = dto.Price,
                Date = dto.Date,
                PaymentMethod = dto.PaymentMethod,
                RideType = dto.RideType
            };

            _context.UserHistories.Add(history);
            await _context.SaveChangesAsync();

            var result = new UserHistoryDto
            {
                Id = history.Id,
                UserId = history.UserId,
                From = history.From,
                To = history.To,
                Price = history.Price,
                Date = history.Date,
                PaymentMethod = history.PaymentMethod,
                RideType = history.RideType
            };

            return Ok(result);
        }

        // GET: api/UserHistory
        [HttpGet]
        public async Task<ActionResult<List<UserHistoryDto>>> GetAllUserHistories()
        {
            var histories = await _context.UserHistories
                .Select(h => new UserHistoryDto
                {
                    Id = h.Id,
                    UserId = h.UserId,
                    From = h.From,
                    To = h.To,
                    Price = h.Price,
                    Date = h.Date,
                    PaymentMethod = h.PaymentMethod,
                    RideType = h.RideType
                })
                .ToListAsync();

            return Ok(histories);
        }

        // GET: api/UserHistory/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<UserHistoryDto>>> GetUserHistoriesByUserId(string userId)
        {
            var histories = await _context.UserHistories
                .Where(h => h.UserId == userId)
                .Select(h => new UserHistoryDto
                {
                    Id = h.Id,
                    UserId = h.UserId,
                    From = h.From,
                    To = h.To,
                    Price = h.Price,
                    Date = h.Date,
                    PaymentMethod = h.PaymentMethod,
                    RideType = h.RideType
                })
                .ToListAsync();

            if (histories == null || histories.Count == 0)
                return NotFound(new ApiResponse(404, "No user history found for this userId"));

            return Ok(histories);
        }

        // GET: api/UserHistory/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserHistoryDto>> GetUserHistoryById(int id)
        {
            var history = await _context.UserHistories.FindAsync(id);
            if (history == null)
                return NotFound(new ApiResponse(404, "User history not found"));

            var dto = new UserHistoryDto
            {
                Id = history.Id,
                UserId = history.UserId,
                From = history.From,
                To = history.To,
                Price = history.Price,
                Date = history.Date,
                PaymentMethod = history.PaymentMethod,
                RideType = history.RideType
            };

            return Ok(dto);
        }
    }
}
