namespace Snap.APIs.DTOs
{
    public class UserDto
    {
        public string UserId { get; set; }
        public string DispalyName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; } 
        public string Token { get; set; }
        public string UserType { get; set; } // Add UserType to response
        public string Gender { get; set; }
    }
}
