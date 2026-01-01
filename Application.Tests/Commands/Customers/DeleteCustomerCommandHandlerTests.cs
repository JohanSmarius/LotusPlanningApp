using Application.Commands.Customers;
using Application.Common;
using Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Application.Tests.Commands.Customers;

public class DeleteCustomerCommandHandlerTests
{
    private readonly Mock<ICustomerRepository> _mockRepository;
    private readonly Mock<ILogger<DeleteCustomerCommandHandler>> _mockLogger;
    private readonly DeleteCustomerCommandHandler _handler;

    public DeleteCustomerCommandHandlerTests()
    {
        _mockRepository = new Mock<ICustomerRepository>();
        _mockLogger = new Mock<ILogger<DeleteCustomerCommandHandler>>();
        _handler = new DeleteCustomerCommandHandler(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_CustomerNotFound_ThrowsApplicationLayerException()
    {
        // Arrange
        var command = new DeleteCustomerCommand(1);
        _mockRepository.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync((Customer?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationLayerException>(() => _handler.Handle(command));
    }

    [Fact]
    public async Task Handle_CustomerHasEvents_ThrowsApplicationLayerException()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Events = new List<Event>
            {
                new Event { Id = 1, Name = "Test Event" }
            }
        };
        var command = new DeleteCustomerCommand(1);
        _mockRepository.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(customer);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApplicationLayerException>(() => _handler.Handle(command));
        Assert.Contains("event(s) associated", exception.Message);
    }

    [Fact]
    public async Task Handle_ValidDelete_DeletesCustomer()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Events = new List<Event>()
        };
        var command = new DeleteCustomerCommand(1);
        _mockRepository.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(customer);
        _mockRepository.Setup(r => r.DeleteCustomerAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteCustomerAsync(1), Times.Once);
    }

    [Fact]
    public async Task Handle_DeleteFails_ReturnsFalse()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Events = new List<Event>()
        };
        var command = new DeleteCustomerCommand(1);
        _mockRepository.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(customer);
        _mockRepository.Setup(r => r.DeleteCustomerAsync(1)).ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.False(result);
    }
}
