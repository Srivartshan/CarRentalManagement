using System.ComponentModel.DataAnnotations;

namespace CarRentalManagement.Models
{
    public class Registration
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{12,}$", 
            ErrorMessage = "Username must have at least 12 characters, including one uppercase letter, one number, and one special character.")]
        public string Username { get; set; }


        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$", 
    ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]

        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
         public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
        [RegularExpression(@"^\+?(?!([0-9])\1{9,14}$)(?!1234567890$)[0-9]{10,15}$", 
            ErrorMessage = "Phone number must be between 10 and 15 digits, can start with '+', and must not be a repetitive or invalid sequence.")]
        public string PhoneNumber { get; set; }

        // [Required(ErrorMessage = "Role is required.")]
        // [StringLength(20, ErrorMessage = "Role cannot exceed 20 characters.")]
        // public string Role { get; set; }


    }
}
