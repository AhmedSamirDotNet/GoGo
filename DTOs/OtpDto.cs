using System.ComponentModel.DataAnnotations;

namespace Snap.APIs.DTOs
{
    public class SendOtpDto
    {
        [Required]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be exactly 11 digits.")]
        public string PhoneNumber { get; set; }
    }

    public class VerifyOtpDto
    {
        [Required]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be exactly 11 digits.")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 4)]
        public string Otp { get; set; }
    }

    public class SendWhatsappOtpDto
    {
        [Required]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be exactly 11 digits.")]
        public string PhoneNumber { get; set; }
    }

    public class VerifyWhatsappOtpDto
    {
        [Required]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be exactly 11 digits.")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 4)]
        public string Otp { get; set; }
    }

    public class ResetPasswordRequestDto
    {
        [Required]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be exactly 11 digits.")]
        public string PhoneNumber { get; set; }
    }

    public class ResetPasswordVerifyOtpDto
    {
        [Required]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be exactly 11 digits.")]
        public string PhoneNumber { get; set; }
        [Required]
        [StringLength(6, MinimumLength = 4)]
        public string Otp { get; set; }
    }

    //public class ResetPasswordDto
    //{
    //    [Required]
    //    [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be exactly 11 digits.")]
    //    public string PhoneNumber { get; set; }
    //    [Required]
    //    [StringLength(6, MinimumLength = 4)]
    //    public string Otp { get; set; }
    //    [Required]
    //    [RegularExpression(
    //        @"^(?=.*[A-Z])(?=.*\d)(?=.*[_\-\W]).{6,20}$",
    //        ErrorMessage = "Password must be 6-20 characters, with at least 1 uppercase letter, 1 number, and 1 special character (e.g., _, -, @, $, etc.)."
    //    )]
    //    public string NewPassword { get; set; }
    //}

    public class ResetPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression(
        @"^(?=.*[A-Z])(?=.*\d)(?=.*[_\-\W]).{6,20}$",
        ErrorMessage = "Password must be 6-20 characters, with at least 1 uppercase letter, 1 number, and 1 special character (e.g., _, -, @, $, etc.)."
    )]
        public string NewPassword { get; set; }
    }
}
