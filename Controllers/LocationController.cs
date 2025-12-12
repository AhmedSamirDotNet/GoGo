//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.SignalR;
//using Snap.APIs.DTOs;
//using Snap.APIs.Hubs;
//using Snap.APIs.Errors;
//using Snap.Repository.Data;
//using Microsoft.EntityFrameworkCore;
//using System.Collections.Concurrent;

//namespace Snap.APIs.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class LocationController : ControllerBase
//    {
//        private readonly IHubContext<LocationHub> _hubContext;
//        private readonly SnapDbContext _context;
//        private static readonly ConcurrentDictionary<int, DriverLocationResponseDto> _driverLocations = new();

//        public LocationController(IHubContext<LocationHub> hubContext, SnapDbContext context)
//        {
//            _hubContext = hubContext;
//            _context = context;
//        }

//        // POST: api/location/update
//        [HttpPost("update")]
//        public async Task<IActionResult> UpdateDriverLocation([FromBody] DriverLocationDto locationDto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            // Verify driver exists
//            var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.Id == locationDto.DriverId);
//            if (driver == null)
//                return NotFound(new ApiResponse(404, "Driver not found"));

//            // Update or create driver location
//            var driverLocation = new DriverLocationResponseDto
//            {
//                DriverId = locationDto.DriverId,
//                DriverName = driver.DriverFullname,
//                Lat = locationDto.Lat,
//                Lng = locationDto.Lng,
//                LastUpdate = locationDto.Timestamp,
//                IsOnline = true
//            };

//            _driverLocations.AddOrUpdate(locationDto.DriverId, driverLocation, (key, oldValue) => driverLocation);

//            // Broadcast location update via SignalR
//            await _hubContext.Clients.Group("Clients").SendAsync("LocationUpdate", driverLocation);
//            await _hubContext.Clients.Group("Drivers").SendAsync("DriverLocationUpdate", driverLocation);

//            return Ok(new { message = "Location updated successfully", location = driverLocation });
//        }

//        // GET: api/location/drivers
//        [HttpGet("drivers")]
//        public IActionResult GetOnlineDrivers()
//        {
//            var onlineDrivers = _driverLocations.Values
//                .Where(d => d.IsOnline && (DateTime.UtcNow - d.LastUpdate).TotalMinutes < 5) // Consider online if updated within 5 minutes
//                .ToList();

//            return Ok(onlineDrivers);
//        }

//        // GET: api/location/driver/{driverId}
//        [HttpGet("driver/{driverId}")]
//        public IActionResult GetDriverLocation(int driverId)
//        {
//            if (_driverLocations.TryGetValue(driverId, out var driverLocation))
//            {
//                // Check if location is recent (within 5 minutes)
//                if ((DateTime.UtcNow - driverLocation.LastUpdate).TotalMinutes < 5)
//                {
//                    return Ok(driverLocation);
//                }
//                else
//                {
//                    driverLocation.IsOnline = false;
//                    return Ok(driverLocation);
//                }
//            }

//            return NotFound(new ApiResponse(404, "Driver location not found"));
//        }

//        // GET: api/location/nearby
//        [HttpGet("nearby")]
//        public IActionResult GetNearbyDrivers([FromQuery] double lat, [FromQuery] double lng, [FromQuery] double radiusKm = 10)
//        {
//            if (lat < -90 || lat > 90)
//                return BadRequest(new ApiResponse(400, "Lat must be between -90 and 90"));

//            if (lng < -180 || lng > 180)
//                return BadRequest(new ApiResponse(400, "Lng must be between -180 and 180"));

//            var nearbyDrivers = _driverLocations.Values
//                .Where(d => d.IsOnline && (DateTime.UtcNow - d.LastUpdate).TotalMinutes < 5)
//                .Where(d => CalculateDistance(lat, lng, d.Lat, d.Lng) <= radiusKm)
//                .OrderBy(d => CalculateDistance(lat, lng, d.Lat, d.Lng))
//                .ToList();

//            return Ok(nearbyDrivers);
//        }

//        // DELETE: api/location/driver/{driverId}
//        [HttpDelete("driver/{driverId}")]
//        public async Task<IActionResult> RemoveDriverLocation(int driverId)
//        {
//            if (_driverLocations.TryRemove(driverId, out var removedLocation))
//            {
//                // Notify clients via SignalR
//                await _hubContext.Clients.All.SendAsync("DriverRemoved", driverId);

//                return Ok(new { message = "Driver location removed", driverId });
//            }

//            return NotFound(new ApiResponse(404, "Driver location not found"));
//        }

//        // Helper method to calculate distance between two points
//        private static double CalculateDistance(double lat1, double lng1, double lat2, double lng2)
//        {
//            const double earthRadius = 6371; // Earth's radius in kilometers

//            var dLat = ToRadians(lat2 - lat1);
//            var dLng = ToRadians(lng2 - lng1);

//            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
//                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
//                    Math.Sin(dLng / 2) * Math.Sin(dLng / 2);

//            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

//            return earthRadius * c;
//        }

//        private static double ToRadians(double degrees)
//        {
//            return degrees * Math.PI / 180;
//        }
//    }
//}