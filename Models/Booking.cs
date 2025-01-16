using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalManagement.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Booking Date is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid date format.")]
        public DateTime BookingDate { get; set; }

        [Required(ErrorMessage = "Return Date is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid date format.")]
        [CustomValidation(typeof(Booking), nameof(ValidateReturnDate))]
        public DateTime ReturnDate { get; set; }

        [Required(ErrorMessage = "Total Price is required.")]
        [Range(0.01, 1000000.00, ErrorMessage = "Total Price must be between 0.01 and 1,000,000.")]
        public decimal TotalPrice { get; set; }

        // Foreign Key for Car
        [Required(ErrorMessage = "Car ID is required.")]
        [ForeignKey("Car")]
        public int CarId { get; set; }

        // Foreign Key for Customer
        [Required(ErrorMessage = "Customer ID is required.")]
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        // Navigation Properties
        public Car Car { get; set; }
        public Customer Customer { get; set; }

        // Custom Validation for ReturnDate
        public static ValidationResult ValidateReturnDate(DateTime returnDate, ValidationContext context)
        {
            if (returnDate < DateTime.Now)
            {
                return new ValidationResult("Return Date cannot be in the past.");
            }

            var instance = context.ObjectInstance as Booking;
            if (instance != null && returnDate <= instance.BookingDate)
            {
                return new ValidationResult("Return Date must be after the Booking Date.");
            }

            return ValidationResult.Success;
        }
    }
}
