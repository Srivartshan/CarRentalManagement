using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarRentalManagement.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Make is required.")]
        [StringLength(50, ErrorMessage = "Make cannot exceed 50 characters.")]
        public string Make { get; set; }

        [Required(ErrorMessage = "Model is required.")]
        [StringLength(50, ErrorMessage = "Model cannot exceed 50 characters.")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Year is required.")]
        [Range(1886, 9999, ErrorMessage = "Year must be a valid four-digit number.")]
        public int Year { get; set; } // First automobile was invented in 1886

        [Required(ErrorMessage = "License Plate is required.")]
        [StringLength(15, MinimumLength = 1, ErrorMessage = "License Plate must be between 1 and 15 characters.")]
        public string LicensePlate { get; set; }

        [Required(ErrorMessage = "Price per day is required.")]
        [Range(0.01, 10000.00, ErrorMessage = "Price per day must be between 0.01 and 10,000.")]
        public decimal PricePerDay { get; set; }

        [Required(ErrorMessage = "Availability status is required.")]
        public bool IsAvailable { get; set; }

        // Navigation Property
        // public ICollection<Booking> Bookings { get; set; }
    }
}
