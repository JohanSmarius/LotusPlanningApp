using Application.Commands.Customers;
using Application.Common;
using Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Application.Tests.Commands.Customers;

/// <summary>
/// Unit tests for LinkCustomerToEventCommandHandler
/// </summary>
public class LinkCustomerToEventCommandHandlerTests
{
    private readonly Mock<ICustomerRepository> _mockCustomerRepository;
    private readonly Mock<IEventRepository> _mockEventRepository;
    private readonly Mock<ILogger<LinkCustomerToEventCommandHandler>> _mockLogger;
    private readonly LinkCustomerToEventCommandHandler _handler;

    public LinkCustomerToEventCommandHandlerTests()
    {
        _mockCustomerRepository = new Mock<ICustomerRepository>();
        _mockEventRepository = new Mock<IEventRepository>();
        _mockLogger = new Mock<ILogger<LinkCustomerToEventCommandHandler>>();
        _handler = new LinkCustomerToEventCommandHandler(
            _mockCustomerRepository.Object,
            _mockEventRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_CustomerNotFound_ThrowsApplicationLayerException()
    {
        // Arrange
        var command = new LinkCustomerToEventCommand(1, 1);
        _mockCustomerRepository.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync((Customer?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApplicationLayerException>(() => _handler.Handle(command));
        Assert.Contains("Customer with ID 1 not found", exception.Message);
    }

    [Fact]
    public async Task Handle_EventNotFound_ThrowsApplicationLayerException()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };
        var command = new LinkCustomerToEventCommand(1, 1);
        _mockCustomerRepository.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(customer);
        _mockEventRepository.Setup(r => r.GetEventByIdAsync(1)).ReturnsAsync((Event?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApplicationLayerException>(() => _handler.Handle(command));
        Assert.Contains("Event with ID 1 not found", exception.Message);
    }

    [Fact]
    public async Task Handle_ValidLinking_LinksCustomerToEvent()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };
        var eventEntity = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location"
        };
        var command = new LinkCustomerToEventCommand(1, 1);
        _mockCustomerRepository.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(customer);
        _mockEventRepository.Setup(r => r.GetEventByIdAsync(1)).ReturnsAsync(eventEntity);
        _mockEventRepository.Setup(r => r.UpdateEventAsync(It.IsAny<Event>())).ReturnsAsync(eventEntity);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result);
        Assert.Equal(1, eventEntity.CustomerId);
        Assert.NotNull(eventEntity.UpdatedAt);
        _mockEventRepository.Verify(r => r.UpdateEventAsync(It.Is<Event>(e => 
            e.CustomerId == 1 && e.UpdatedAt != null)), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidLinking_LogsInformation()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };
        var eventEntity = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location"
        };
        var command = new LinkCustomerToEventCommand(1, 1);
        _mockCustomerRepository.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(customer);
        _mockEventRepository.Setup(r => r.GetEventByIdAsync(1)).ReturnsAsync(eventEntity);
        _mockEventRepository.Setup(r => r.UpdateEventAsync(It.IsAny<Event>())).ReturnsAsync(eventEntity);

        // Act
        await _handler.Handle(command);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Customer 1 linked to Event 1")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_DifferentCustomerIds_ThrowsCorrectException()
    {
        // Arrange
        var command = new LinkCustomerToEventCommand(999, 1);
        _mockCustomerRepository.Setup(r => r.GetCustomerByIdAsync(999)).ReturnsAsync((Customer?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApplicationLayerException>(() => _handler.Handle(command));
        Assert.Contains("Customer with ID 999 not found", exception.Message);
    }

    [Fact]
    public async Task Handle_DifferentEventIds_ThrowsCorrectException()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };
        var command = new LinkCustomerToEventCommand(1, 999);
        _mockCustomerRepository.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(customer);
        _mockEventRepository.Setup(r => r.GetEventByIdAsync(999)).ReturnsAsync((Event?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApplicationLayerException>(() => _handler.Handle(command));
        Assert.Contains("Event with ID 999 not found", exception.Message);
    }

    [Fact]
    public async Task Handle_ValidLinking_UpdatesEventTimestamp()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };
        var eventEntity = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            UpdatedAt = null
        };
        var command = new LinkCustomerToEventCommand(1, 1);
        _mockCustomerRepository.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(customer);
        _mockEventRepository.Setup(r => r.GetEventByIdAsync(1)).ReturnsAsync(eventEntity);
        _mockEventRepository.Setup(r => r.UpdateEventAsync(It.IsAny<Event>())).ReturnsAsync(eventEntity);

        // Act
        await _handler.Handle(command);

        // Assert
        Assert.NotNull(eventEntity.UpdatedAt);
        Assert.True(eventEntity.UpdatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public async Task Handle_ValidLinking_CallsRepositoryMethodsInOrder()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };
        var eventEntity = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location"
        };
        var command = new LinkCustomerToEventCommand(1, 1);
        _mockCustomerRepository.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(customer);
        _mockEventRepository.Setup(r => r.GetEventByIdAsync(1)).ReturnsAsync(eventEntity);
        _mockEventRepository.Setup(r => r.UpdateEventAsync(It.IsAny<Event>())).ReturnsAsync(eventEntity);

        // Act
        await _handler.Handle(command);

        // Assert
        _mockCustomerRepository.Verify(r => r.GetCustomerByIdAsync(1), Times.Once);
        _mockEventRepository.Verify(r => r.GetEventByIdAsync(1), Times.Once);
        _mockEventRepository.Verify(r => r.UpdateEventAsync(It.IsAny<Event>()), Times.Once);
    }
}
