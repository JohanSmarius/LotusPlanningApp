using Application.Commands.Customers;
using Application.Common;
using Application.DataAdapters;
using Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Application.Tests.Commands.Customers;

public class UpdateCustomerCommandHandlerTests
{
    private readonly Mock<ICustomerRepository> _mockRepository;
    private readonly Mock<ILogger<UpdateCustomerCommandHandler>> _mockLogger;
    private readonly UpdateCustomerCommandHandler _handler;

    public UpdateCustomerCommandHandlerTests()
    {
        _mockRepository = new Mock<ICustomerRepository>();
        _mockLogger = new Mock<ILogger<UpdateCustomerCommandHandler>>();
        _handler = new UpdateCustomerCommandHandler(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_CustomerNotFound_ThrowsApplicationLayerException()
    {
        // Arrange
        var customerDto = new CustomerDTO { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" };
        var command = new UpdateCustomerCommand(customerDto);
        _mockRepository.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync((Customer?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationLayerException>(() => _handler.Handle(command));
    }

    [Fact]
    public async Task Handle_EmailConflict_ThrowsApplicationLayerException()
    {
        // Arrange
        var existingCustomer = new Customer { Id = 1, FirstName = "John", LastName = "Doe", Email = "old@example.com" };
        var conflictingCustomer = new Customer { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "new@example.com" };
        var customerDto = new CustomerDTO { Id = 1, FirstName = "John", LastName = "Doe", Email = "new@example.com" };
        var command = new UpdateCustomerCommand(customerDto);

        _mockRepository.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(existingCustomer);
        _mockRepository.Setup(r => r.GetCustomerByEmailAsync("new@example.com")).ReturnsAsync(conflictingCustomer);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApplicationLayerException>(() => _handler.Handle(command));
        Assert.Contains("already exists", exception.Message);
    }

    [Fact]
    public async Task Handle_ValidUpdate_UpdatesCustomer()
    {
        // Arrange
        var existingCustomer = new Customer { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" };
        var customerDto = new CustomerDTO { Id = 1, FirstName = "John", LastName = "Updated", Email = "john@example.com" };
        var command = new UpdateCustomerCommand(customerDto);

        _mockRepository.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(existingCustomer);
        _mockRepository.Setup(r => r.UpdateCustomerAsync(It.IsAny<Customer>())).ReturnsAsync(existingCustomer);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated", result.LastName);
        _mockRepository.Verify(r => r.UpdateCustomerAsync(It.IsAny<Customer>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EmailUnchanged_DoesNotCheckForConflict()
    {
        // Arrange
        var existingCustomer = new Customer { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" };
        var customerDto = new CustomerDTO { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" };
        var command = new UpdateCustomerCommand(customerDto);

        _mockRepository.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(existingCustomer);
        _mockRepository.Setup(r => r.UpdateCustomerAsync(It.IsAny<Customer>())).ReturnsAsync(existingCustomer);

        // Act
        await _handler.Handle(command);

        // Assert
        _mockRepository.Verify(r => r.GetCustomerByEmailAsync(It.IsAny<string>()), Times.Never);
    }
}
