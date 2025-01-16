using Microsoft.EntityFrameworkCore;
using CarRentalManagementAPI.Models;
using CarRentalManagement.Models;

namespace CarRentalManagementAPI.Data
{
    public class CarRentalContext : DbContext
    {
        public CarRentalContext(DbContextOptions<CarRentalContext> options) : base(options) { }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Customer> Customers { get; set; }
    }
}
