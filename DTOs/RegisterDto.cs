using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Snap.APIs.DTOs
{
    public class RegisterDto
    {

        [Required]
        [RegularExpression(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Invalid email format. Please enter a valid email like example@mail.com."
        )]
        public string Email { get; set; }
        [Required]
        public string FullName { get; set; }

        [Required]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be exactly 11 digits.")]
        public string PhoneNumber { get; set; }

        [Required]
        [RegularExpression(
            @"^(?=.*[A-Z])(?=.*\d)(?=.*[_\-\W]).{6,20}$",
            ErrorMessage = "Password must be 6-20 characters, with at least 1 uppercase letter, 1 number, and 1 special character (e.g., _, -, @, $, etc.)."
        )]
        [JsonPropertyName("password")] 
        public string password { get; set; }

        [Required]
        [RegularExpression("^(driver|passenger)$", ErrorMessage = "UserType must be either 'driver' or 'passenger'.")]
        public string UserType { get; set; }
        [Required]
        public string Gender { get; set; }
    }
}
