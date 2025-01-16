using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRentalManagement.Models;
using CarRentalManagementAPI.Data;
using System.Threading.Tasks;

namespace CarRentalManagement.Controllers
{
    [Route("api/Car")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly CarRentalContext _context;

        public CarController(CarRentalContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCars()
        {
            var cars = await _context.Cars.ToListAsync();
            return Ok(cars);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCar(int id)
        {
            var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == id); 
            if (car == null) return NotFound();

            return Ok(car);
        }

        [HttpPost]
        public async Task<IActionResult> AddCar(Car car)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Check if a car with the same registration number or unique field exists
            var existingCar = await _context.Cars.FirstOrDefaultAsync(c => c.LicensePlate == car.LicensePlate);
            if (existingCar != null)
                return Conflict(new { message = "A car with this registration number already exists." });

            await _context.Cars.AddAsync(car);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCar), new { id = car.Id }, car);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCar(int id, Car car)
        {
            if (id != car.Id) return BadRequest();

            // Use FirstOrDefaultAsync to check if the car exists before updating
            var existingCar = await _context.Cars.FirstOrDefaultAsync(c => c.Id == id);
            if (existingCar == null) return NotFound();

            // Update the car details
            existingCar.Model = car.Model;
            existingCar.LicensePlate = car.LicensePlate;
            existingCar.Make = car.Make;
            existingCar.Year = car.Year;
            existingCar.PricePerDay = car.PricePerDay;
            existingCar.IsAvailable = car.IsAvailable;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == id); // Using FirstOrDefaultAsync
            if (car == null) return NotFound();

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
