using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRentalManagement.Models;
using CarRentalManagementAPI.Data;
using System.Threading.Tasks;

namespace CarRentalManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CarRentalContext _context;

        public CustomerController(CarRentalContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _context.Customers.ToListAsync();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
            if (customer == null) return NotFound();

            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(Customer customer)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Check if a customer with the same email already exists
            var existingCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == customer.Email);
            if (existingCustomer != null)
                return Conflict(new { message = "A customer with this email already exists." });

            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, Customer customer)
        {
            if (id != customer.Id) return BadRequest();

            // Use FirstOrDefaultAsync to check if the customer exists
            var existingCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
            if (existingCustomer == null) return NotFound();

            // Update the customer details
            existingCustomer.FirstName = customer.FirstName;
            existingCustomer.LastName = customer.LastName;
            existingCustomer.Email = customer.Email;
            existingCustomer.Phone = customer.Phone;
            existingCustomer.Address = customer.Address;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id); // Using FirstOrDefaultAsync
            if (customer == null) return NotFound();

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
