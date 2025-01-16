using Xunit;
using CarRentalManagement.Controllers;
using CarRentalManagement.Models;
using CarRentalManagementAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CarRentalManagement.Tests
{
    public class CarControllerTests
    {
        private readonly CarRentalContext _context;
        private readonly CarController _controller;

        public CarControllerTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<CarRentalContext>()
                .UseInMemoryDatabase(databaseName: "CarRentalTestDb")
                .Options;

            _context = new CarRentalContext(options);

            // Seed data
            if (!_context.Cars.Any())
            {
                _context.Cars.AddRange(new List<Car>
                {
                    new Car { Id = 1, Make = "Toyota", Model = "Corolla", Year = 2021, LicensePlate = "ABC123", PricePerDay = 50, IsAvailable = true },
                    new Car { Id = 2, Make = "Ford", Model = "Mustang", Year = 2020, LicensePlate = "XYZ789", PricePerDay = 100, IsAvailable = false }
                });
                _context.SaveChanges();
            }

            _controller = new CarController(_context);
        }

        [Fact]
        public async Task GetCars_Should_Return_All_Cars()
        {
            var result = await _controller.GetCars();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var cars = Assert.IsType<List<Car>>(okResult.Value);

            Assert.Equal(2, cars.Count);
        }

        [Fact]
        public async Task GetCar_Should_Return_Correct_Car()
        {
            var result = await _controller.GetCar(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var car = Assert.IsType<Car>(okResult.Value);

            Assert.Equal(1, car.Id);
            Assert.Equal("Toyota", car.Make);
        }

        [Fact]
        public async Task GetCar_Should_Return_NotFound_For_Invalid_Id()
        {
            var result = await _controller.GetCar(99);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddCar_Should_Add_New_Car()
        {
            var newCar = new Car
            {
                Make = "Honda",
                Model = "Civic",
                Year = 2022,
                LicensePlate = "LMN456",
                PricePerDay = 75,
                IsAvailable = true
            };

            var result = await _controller.AddCar(newCar);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var car = Assert.IsType<Car>(createdResult.Value);

            Assert.Equal("Honda", car.Make);
            Assert.Equal(3, car.Id); // New car's ID
        }

        [Fact]
        public async Task AddCar_Should_Return_Conflict_For_Duplicate_LicensePlate()
        {
            var duplicateCar = new Car
            {
                Make = "Tesla",
                Model = "Model 3",
                Year = 2023,
                LicensePlate = "ABC123",
                PricePerDay = 120,
                IsAvailable = true
            };

            var result = await _controller.AddCar(duplicateCar);
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);

            Assert.Equal("A car with this registration number already exists.", conflictResult.Value.GetType().GetProperty("message").GetValue(conflictResult.Value));
        }

        [Fact]
        public async Task UpdateCar_Should_Modify_Existing_Car()
        {
            var updatedCar = new Car
            {
                Id = 1,
                Make = "Toyota",
                Model = "Camry",
                Year = 2022,
                LicensePlate = "ABC123",
                PricePerDay = 60,
                IsAvailable = true
            };

            var result = await _controller.UpdateCar(1, updatedCar);
            Assert.IsType<NoContentResult>(result);

            var car = await _context.Cars.FindAsync(1);
            Assert.Equal("Camry", car.Model);
            Assert.Equal(60, car.PricePerDay);
        }

        [Fact]
        public async Task DeleteCar_Should_Remove_Car()
        {
            var result = await _controller.DeleteCar(1);
            Assert.IsType<NoContentResult>(result);

            var car = await _context.Cars.FindAsync(1);
            Assert.Null(car); // Car should be deleted
        }
    }

    public class CarModelTests
    {
        [Fact]
        public void CarModel_Should_Validate_RequiredFields()
        {
            var car = new Car(); // Empty object
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(car);

            var isValid = Validator.TryValidateObject(car, context, validationResults, true);

            Assert.False(isValid); // Should fail validation
            Assert.Contains(validationResults, v => v.ErrorMessage.Contains("Make is required."));
            Assert.Contains(validationResults, v => v.ErrorMessage.Contains("Model is required."));
            Assert.Contains(validationResults, v => v.ErrorMessage.Contains("Year is required."));
            Assert.Contains(validationResults, v => v.ErrorMessage.Contains("License Plate is required."));
            Assert.Contains(validationResults, v => v.ErrorMessage.Contains("Price per day is required."));
        }
    }
}
