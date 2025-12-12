namespace Snap.APIs.DTOs
{
    public class CreateDriver
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
    }
}
