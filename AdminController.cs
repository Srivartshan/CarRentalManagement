using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using CarRentalManagementAPI.Data;
using CarRentalManagementAPI.Controllers;
using CarRentalManagementAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CarRentalManagementAPI.Tests
{
    public class AdminControllerTests
    {
        private readonly AdminController _controller;
        private readonly CarRentalContext _context;

        public AdminControllerTests()
        {
            var options = new DbContextOptionsBuilder<CarRentalContext>()
                .UseInMemoryDatabase(databaseName: "CarRentalTestDB")
                .Options;

            _context = new CarRentalContext(options);
            _controller = new AdminController(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            if (!_context.Admins.Any())
            {
                _context.Admins.AddRange(new List<Admin>
                {
                    new Admin { Id = 1, Username = "admin1", Password = "Pass@123" },
                    new Admin { Id = 2, Username = "admin2", Password = "Pass@123" }
                });
                _context.SaveChanges();
            }
        }

        [Fact]
        public async Task GetAll_ReturnsAllAdmins()
        {
            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var admins = Assert.IsAssignableFrom<IEnumerable<Admin>>(okResult.Value);
            Assert.Equal(2, admins.Count());
        }

        [Fact]
        public async Task GetById_ValidId_ReturnsAdmin()
        {
            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var admin = Assert.IsType<Admin>(okResult.Value);
            Assert.Equal("admin1", admin.Username);
        }

        [Fact]
        public async Task GetById_InvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetById(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ValidAdmin_ReturnsCreatedAdmin()
        {
            // Arrange
            var newAdmin = new Admin
            {
                Username = "admin3",
                Password = "Pass@123"
            };

            // Act
            var result = await _controller.Create(newAdmin);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var admin = Assert.IsType<Admin>(createdResult.Value);
            Assert.Equal("admin3", admin.Username);
        }

        [Fact]
        public async Task Update_ValidId_UpdatesAdmin()
        {
            // Arrange
            var adminToUpdate = new Admin
            {
                Id = 1,
                Username = "updatedAdmin",
                Password = "NewPass@123"
            };

            // Act
            var result = await _controller.Update(1, adminToUpdate);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var updatedAdmin = await _context.Admins.FindAsync(1);
            Assert.Equal("updatedAdmin", updatedAdmin.Username);
        }

        [Fact]
        public async Task Delete_ValidId_DeletesAdmin()
        {
            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(await _context.Admins.FindAsync(1));
        }
    }

    public class AdminModelTests
    {
        [Fact]
        public void AdminModel_ValidationPasses()
        {
            // Arrange
            var admin = new Admin
            {
                Username = "validUser",
                Password = "Valid@123"
            };

            var context = new ValidationContext(admin, null, null);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(admin, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void AdminModel_InvalidPasswordFailsValidation()
        {
            // Arrange
            var admin = new Admin
            {
                Username = "validUser",
                Password = "short"
            };

            var context = new ValidationContext(admin, null, null);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(admin, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.NotEmpty(results);
            Assert.Contains(results, r => r.ErrorMessage.Contains("Password must contain"));
        }
    }
}
