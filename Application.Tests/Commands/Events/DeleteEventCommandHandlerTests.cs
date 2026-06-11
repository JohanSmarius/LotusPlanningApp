using Application.Commands.Events;
using Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Application.Tests.Commands.Events;

public class DeleteEventCommandHandlerTests
{
    private readonly Mock<IEventRepository> _mockRepository;
    private readonly Mock<ILogger<DeleteEventCommandHandler>> _mockLogger;
    private readonly DeleteEventCommandHandler _handler;

    public DeleteEventCommandHandlerTests()
    {
        _mockRepository = new Mock<IEventRepository>();
        _mockLogger = new Mock<ILogger<DeleteEventCommandHandler>>();
        _handler = new DeleteEventCommandHandler(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_InitializesHandler()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockLogger = new Mock<ILogger<DeleteEventCommandHandler>>();

        // Act
        var handler = new DeleteEventCommandHandler(mockRepository.Object, mockLogger.Object);

        // Assert
        Assert.NotNull(handler);
    }

    [Fact]
    public async Task Handle_EventExists_DeletesEventAndReturnsTrue()
    {
        // Arrange
        var eventId = 1;
        var command = new DeleteEventCommand(eventId);

        var existingEvent = new Event
        {
            Id = eventId,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.Requested
        };

        var allEvents = new List<Event>
        {
            existingEvent,
            new Event { Id = 2, Name = "Other Event", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), Location = "Other Location" }
        };

        _mockRepository.Setup(r => r.GetEventByIdAsync(eventId)).ReturnsAsync(existingEvent);
        _mockRepository.Setup(r => r.GetAllEventsAsync()).ReturnsAsync(allEvents);
        _mockRepository.Setup(r => r.DeleteEventAsync(eventId)).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.GetEventByIdAsync(eventId), Times.Once);
        _mockRepository.Verify(r => r.GetAllEventsAsync(), Times.Once);
        _mockRepository.Verify(r => r.DeleteEventAsync(eventId), Times.Once);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Event {eventId} deleted successfully")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_EventNotFound_ReturnsFalseAndLogsWarning()
    {
        // Arrange
        var eventId = 999;
        var command = new DeleteEventCommand(eventId);

        var allEvents = new List<Event>
        {
            new Event { Id = 1, Name = "Event 1", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), Location = "Location 1" },
            new Event { Id = 2, Name = "Event 2", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), Location = "Location 2" }
        };

        _mockRepository.Setup(r => r.GetEventByIdAsync(eventId)).ReturnsAsync((Event?)null);
        _mockRepository.Setup(r => r.GetAllEventsAsync()).ReturnsAsync(allEvents);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.False(result);
        _mockRepository.Verify(r => r.GetEventByIdAsync(eventId), Times.Once);
        _mockRepository.Verify(r => r.GetAllEventsAsync(), Times.Once);
        _mockRepository.Verify(r => r.DeleteEventAsync(It.IsAny<int>()), Times.Never);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Attempted to delete non-existent event {eventId}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyEventList_ReturnsFalseAndLogsWarning()
    {
        // Arrange
        var eventId = 1;
        var command = new DeleteEventCommand(eventId);

        var emptyEvents = new List<Event>();

        _mockRepository.Setup(r => r.GetEventByIdAsync(eventId)).ReturnsAsync((Event?)null);
        _mockRepository.Setup(r => r.GetAllEventsAsync()).ReturnsAsync(emptyEvents);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.False(result);
        _mockRepository.Verify(r => r.DeleteEventAsync(It.IsAny<int>()), Times.Never);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Attempted to delete non-existent event")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_EventExistsAsFirstInList_DeletesEventSuccessfully()
    {
        // Arrange
        var eventId = 1;
        var command = new DeleteEventCommand(eventId);

        var targetEvent = new Event
        {
            Id = eventId,
            Name = "First Event",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            Location = "Location"
        };

        var allEvents = new List<Event>
        {
            targetEvent,
            new Event { Id = 2, Name = "Second Event", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), Location = "Location 2" },
            new Event { Id = 3, Name = "Third Event", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), Location = "Location 3" }
        };

        _mockRepository.Setup(r => r.GetEventByIdAsync(eventId)).ReturnsAsync(targetEvent);
        _mockRepository.Setup(r => r.GetAllEventsAsync()).ReturnsAsync(allEvents);
        _mockRepository.Setup(r => r.DeleteEventAsync(eventId)).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteEventAsync(eventId), Times.Once);
    }

    [Fact]
    public async Task Handle_EventExistsAsLastInList_DeletesEventSuccessfully()
    {
        // Arrange
        var eventId = 3;
        var command = new DeleteEventCommand(eventId);

        var targetEvent = new Event
        {
            Id = eventId,
            Name = "Last Event",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            Location = "Location"
        };

        var allEvents = new List<Event>
        {
            new Event { Id = 1, Name = "First Event", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), Location = "Location 1" },
            new Event { Id = 2, Name = "Second Event", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), Location = "Location 2" },
            targetEvent
        };

        _mockRepository.Setup(r => r.GetEventByIdAsync(eventId)).ReturnsAsync(targetEvent);
        _mockRepository.Setup(r => r.GetAllEventsAsync()).ReturnsAsync(allEvents);
        _mockRepository.Setup(r => r.DeleteEventAsync(eventId)).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteEventAsync(eventId), Times.Once);
    }

    [Fact]
    public async Task Handle_SingleEventInList_DeletesSuccessfully()
    {
        // Arrange
        var eventId = 1;
        var command = new DeleteEventCommand(eventId);

        var singleEvent = new Event
        {
            Id = eventId,
            Name = "Only Event",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            Location = "Location"
        };

        var allEvents = new List<Event> { singleEvent };

        _mockRepository.Setup(r => r.GetEventByIdAsync(eventId)).ReturnsAsync(singleEvent);
        _mockRepository.Setup(r => r.GetAllEventsAsync()).ReturnsAsync(allEvents);
        _mockRepository.Setup(r => r.DeleteEventAsync(eventId)).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteEventAsync(eventId), Times.Once);
    }

    [Fact]
    public async Task Handle_WithCancellationToken_PassesTokenCorrectly()
    {
        // Arrange
        var eventId = 1;
        var command = new DeleteEventCommand(eventId);
        var cancellationToken = new CancellationToken();

        var existingEvent = new Event
        {
            Id = eventId,
            Name = "Test Event",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            Location = "Location"
        };

        var allEvents = new List<Event> { existingEvent };

        _mockRepository.Setup(r => r.GetEventByIdAsync(eventId)).ReturnsAsync(existingEvent);
        _mockRepository.Setup(r => r.GetAllEventsAsync()).ReturnsAsync(allEvents);
        _mockRepository.Setup(r => r.DeleteEventAsync(eventId)).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task Handle_MultipleEventsWithSameProperties_FindsCorrectEventById()
    {
        // Arrange
        var eventId = 2;
        var command = new DeleteEventCommand(eventId);

        var allEvents = new List<Event>
        {
            new Event { Id = 1, Name = "Duplicate Name", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), Location = "Same Location" },
            new Event { Id = 2, Name = "Duplicate Name", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), Location = "Same Location" },
            new Event { Id = 3, Name = "Duplicate Name", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), Location = "Same Location" }
        };

        _mockRepository.Setup(r => r.GetEventByIdAsync(eventId)).ReturnsAsync(allEvents[1]);
        _mockRepository.Setup(r => r.GetAllEventsAsync()).ReturnsAsync(allEvents);
        _mockRepository.Setup(r => r.DeleteEventAsync(eventId)).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteEventAsync(eventId), Times.Once);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Event {eventId} deleted successfully")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
