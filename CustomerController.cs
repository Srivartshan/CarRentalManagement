using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarRentalManagement.Controllers;
using CarRentalManagement.Models;
using CarRentalManagementAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

public class CustomerControllerTests
{
    private readonly DbContextOptions<CarRentalContext> _dbContextOptions;

    public CustomerControllerTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<CarRentalContext>()
            .UseInMemoryDatabase(databaseName: "CarRentalTestDb")
            .Options;
    }

    [Fact]
    public async Task GetCustomers_ShouldReturnAllCustomers()
    {
        // Arrange
        using var context = new CarRentalContext(_dbContextOptions);
        context.Customers.AddRange(
            new Customer { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", Phone = 1234567890, Address = "123 Main St" },
            new Customer { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", Phone = 9876543210, Address = "456 Main St" }
        );
        await context.SaveChangesAsync();

        var controller = new CustomerController(context);

        // Act
        var result = await controller.GetCustomers() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var customers = result.Value as List<Customer>;
        Assert.Equal(2, customers.Count);
    }

    [Fact]
    public async Task GetCustomer_ShouldReturnCustomerById()
    {
        // Arrange
        using var context = new CarRentalContext(_dbContextOptions);
        var customer = new Customer { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", Phone = 1234567890, Address = "123 Main St" };
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var controller = new CustomerController(context);

        // Act
        var result = await controller.GetCustomer(1) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customer, result.Value);
    }

    [Fact]
    public async Task AddCustomer_ShouldAddCustomerSuccessfully()
    {
        // Arrange
        using var context = new CarRentalContext(_dbContextOptions);
        var controller = new CustomerController(context);
        var newCustomer = new Customer { FirstName = "Alice", LastName = "Johnson", Email = "alice@example.com", Phone = 1234567890, Address = "789 Main St" };

        // Act
        var result = await controller.AddCustomer(newCustomer) as CreatedAtActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newCustomer.Email, ((Customer)result.Value).Email);
        Assert.Single(context.Customers.ToList());
    }

    [Fact]
    public async Task DeleteCustomer_ShouldRemoveCustomer()
    {
        // Arrange
        using var context = new CarRentalContext(_dbContextOptions);
        var customer = new Customer { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", Phone = 1234567890, Address = "123 Main St" };
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var controller = new CustomerController(context);

        // Act
        var result = await controller.DeleteCustomer(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Empty(context.Customers.ToList());
    }
}
