using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CarRentalManagement.Attributes;

namespace CarRentalManagement.Models
{
    public class Booking
{
    public int Id { get; set; }

   [Required(ErrorMessage = "Car ID is required.")]
    [ForeignKey("Car")]
    public int CarId { get; set; }

    public Car? Car { get; set; }

    [Required(ErrorMessage = "Customer ID is required.")]
    [ForeignKey("Customer")]
    public int CustomerId { get; set; }

    public Customer? Customer { get; set; }

    [Required(ErrorMessage = "Booking date is required.")]
    [DataType(DataType.DateTime)]
    [Display(Name = "Booking Date")]
    public DateTime BookingDate { get; set; }

    [Required(ErrorMessage = "Return date is required.")]
    [DataType(DataType.DateTime)]
    [Display(Name = "Return Date")]
    [DateGreaterThan(nameof(BookingDate), ErrorMessage = "Return date must be after the booking date.")]
    public DateTime ReturnDate { get; set; }
    
}
}
