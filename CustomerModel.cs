using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CarRentalManagement.Models;
using Xunit;

public class CustomerModelTests
{
    [Fact]
    public void ValidateCustomer_ValidData_ShouldPassValidation()
    {
        // Arrange
        var customer = new Customer
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Phone = 9865872345,
            Address = "123 Main St"
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(customer, new ValidationContext(customer), validationResults, true);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void ValidateCustomer_InvalidEmail_ShouldFailValidation()
    {
        // Arrange
        var customer = new Customer
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "invalid-email",
            Phone = 9865872345,
            Address = "123 Main St"
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(customer, new ValidationContext(customer), validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.ErrorMessage == "Invalid Email Address.");
    }
}
