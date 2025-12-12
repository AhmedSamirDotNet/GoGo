using System;

namespace Snap.APIs.DTOs
{
    public class DriverDto
    {
        public int Id { get; set; }
        public string DriverPhoto { get; set; }
        public string DriverIdCard { get; set; }
        public string DriverLicenseFront { get; set; }
        public string DriverLicenseBack { get; set; }
        public string IdCardFront { get; set; }
        public string IdCardBack { get; set; }
        public string DriverFullname { get; set; }
        public string NationalId { get; set; }
        public int Age { get; set; }
        public string LicenseNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime LicenseExpiryDate { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public double Review { get; set; }
        public double Wallet { get; set; }
        public string Gender { get; set; } // Added
        public string CarBrand { get; set; } // Added
        public string? PhoneNumber { get; set; }
    }

    public class DriverIdDto
    {
        public int Id { get; set; }
        public string DriverPhoto { get; set; }
        public string DriverFullname { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public double Review { get; set; }
        public int? PhoneNumber { get; set; }
        public string Gender { get; set; } // Added
        public string CarBrand { get; set; } // Added
    }

    public class ApprovedDriverWithCarDto
    {
        public int DriverId { get; set; }
        public string DriverPhoto { get; set; }
        public string DriverFullname { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public string LicenseNumber { get; set; }
        public DateTime LicenseExpiryDate { get; set; }
        public string UserId { get; set; }
        public double Review { get; set; }
        public double Wallet { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        
        // Car Data
        public int? CarId { get; set; }
        public string CarPhoto { get; set; }
        public string CarLicenseFront { get; set; }
        public string CarLicenseBack { get; set; }
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public string CarColor { get; set; }
        public string PlateNumber { get; set; }
    }
}