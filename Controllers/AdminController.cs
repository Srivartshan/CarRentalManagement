using Microsoft.AspNetCore.Mvc;
using CarRentalManagementAPI.Data;
using CarRentalManagementAPI.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CarRentalManagementAPI.Controllers
{
    [Route("api/Admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly CarRentalContext _context;

        public AdminController(CarRentalContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var admins = await _context.Admins.ToListAsync();
            return Ok(admins);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Admin admin)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _context.Admins.AddAsync(admin); // Using AddAsync
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = admin.Id }, admin);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Id == id);
            if (admin == null)
                return NotFound();

            return Ok(admin);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Admin admin)
        {
            if (id != admin.Id)
                return BadRequest();

            _context.Entry(admin).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Admins.AnyAsync(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Id == id);
            if (admin == null)
                return NotFound();

            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
