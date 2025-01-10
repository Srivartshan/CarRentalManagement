using System;
using System.ComponentModel.DataAnnotations;

namespace CarRentalManagement.Models
{
    public class Car
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Model is required.")]
        [StringLength(100, ErrorMessage = "Model name cannot exceed 100 characters.")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Make is required.")]
        [StringLength(50, ErrorMessage = "Make name cannot exceed 50 characters.")]
        public string Make { get; set; }

        [Required(ErrorMessage = "Year is required.")]
        [Range(1886, int.MaxValue, ErrorMessage = "Year must be 1886 or later.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Daily rate is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Daily rate must be a positive value.")]
        public double DailyRate { get; set; }

        [Required(ErrorMessage = "Seating capacity is required.")]
        [Range(1, 100, ErrorMessage = "Seating capacity must be between 1 and 100.")]
        public int SeatingCapacity { get; set; }

        [Required(ErrorMessage = "Fuel type is required.")]
        [StringLength(20, ErrorMessage = "Fuel type cannot exceed 20 characters.")]
        public string FuelType { get; set; }
    }
}
