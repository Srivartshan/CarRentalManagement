using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRentalManagement.Models;
using CarRentalManagementAPI.Data;
using System.Threading.Tasks;

namespace CarRentalManagement.Controllers
{
    [Route("api/Booking")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly CarRentalContext _context;

        public BookingController(CarRentalContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Customer)
                .ToListAsync();
            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Customer)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            return Ok(booking);
        }

        [HttpPost]
        public async Task<IActionResult> AddBooking(Booking booking)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Optional: Check if the booking already exists using FirstOrDefaultAsync
            var existingBooking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.CarId == booking.CarId && b.CustomerId == booking.CustomerId && b.BookingDate == booking.BookingDate && b.ReturnDate == booking.ReturnDate);

            if (existingBooking != null)
                return Conflict(new { message = "This booking already exists." });

            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, Booking booking)
        {
            if (id != booking.Id) return BadRequest();

            // Use FirstOrDefaultAsync to check if the booking exists before updating
            var existingBooking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);
            if (existingBooking == null) return NotFound();

            // Update the booking details
            existingBooking.CarId = booking.CarId;
            existingBooking.CustomerId = booking.CustomerId;
            existingBooking.BookingDate = booking.BookingDate;
            existingBooking.ReturnDate = booking.ReturnDate;
            existingBooking.TotalPrice = booking.TotalPrice;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id); // Using FirstOrDefaultAsync
            if (booking == null) return NotFound();

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
