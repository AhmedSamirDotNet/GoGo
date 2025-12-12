using System;

namespace Snap.APIs.DTOs
{
    public class CarDataDto
    {
        public int Id { get; set; }
        public string CarPhoto { get; set; }
        public string LicenseFront { get; set; }
        public string LicenseBack { get; set; }
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public string CarColor { get; set; }
        public string PlateNumber { get; set; }
        public int DriverId { get; set; }
    }
}
