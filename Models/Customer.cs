using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarRentalManagement.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Range(1000000000, 999999999999, ErrorMessage = "Phone number must be between 10 and 12 digits.")]
        public long Phone { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(250, MinimumLength = 10, ErrorMessage = "Address must be between 10 and 250 characters.")]
        public string Address { get; set; }

        // Navigation Property
        // public ICollection<Booking> Bookings { get; set; }
    }
}
