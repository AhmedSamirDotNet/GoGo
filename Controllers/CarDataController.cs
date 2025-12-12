using Microsoft.AspNetCore.Mvc;
using Snap.Core.Entities;
using Snap.APIs.DTOs;
using Snap.Repository.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Snap.APIs.Errors;

namespace Snap.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarDataController : ControllerBase
    {
        private readonly SnapDbContext _context;
        public CarDataController(SnapDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCarData([FromBody] CarDataDto dto)
        {
            var carData = new CarData
            {
                CarPhoto = dto.CarPhoto,
                LicenseFront = dto.LicenseFront,
                LicenseBack = dto.LicenseBack,
                CarBrand = dto.CarBrand,
                CarModel = dto.CarModel,
                CarColor = dto.CarColor,
                PlateNumber = dto.PlateNumber,
                DriverId = dto.DriverId
            };
            _context.CarDatas.Add(carData);
            await _context.SaveChangesAsync();
            dto.Id = carData.Id;
            return Ok(dto);
        }

        [HttpGet("by-driver/{driverId}")]
        public async Task<ActionResult<CarDataDto>> GetCarDataByDriverId(int driverId)
        {
            var carData = await _context.CarDatas.FirstOrDefaultAsync(c => c.DriverId == driverId);
            if (carData == null) return NotFound(new ApiResponse(404, "Car data not found"));
            var dto = new CarDataDto
            {
                Id = carData.Id,
                CarPhoto = carData.CarPhoto,
                LicenseFront = carData.LicenseFront,
                LicenseBack = carData.LicenseBack,
                CarBrand = carData.CarBrand,
                CarModel = carData.CarModel,
                CarColor = carData.CarColor,
                PlateNumber = carData.PlateNumber,
                DriverId = carData.DriverId
            };
            return Ok(dto);
        }
    }
}
