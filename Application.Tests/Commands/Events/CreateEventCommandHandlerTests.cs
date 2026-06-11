using Application.Commands.Events;
using Application.DataAdapters;
using Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Application.Tests.Commands.Events;

public class CreateEventCommandHandlerTests
{
    private readonly Mock<IEventRepository> _mockRepository;
    private readonly Mock<ILogger<CreateEventCommandHandler>> _mockLogger;
    private readonly CreateEventCommandHandler _handler;

    public CreateEventCommandHandlerTests()
    {
        _mockRepository = new Mock<IEventRepository>();
        _mockLogger = new Mock<ILogger<CreateEventCommandHandler>>();
        _handler = new CreateEventCommandHandler(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_InitializesHandler()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockLogger = new Mock<ILogger<CreateEventCommandHandler>>();

        // Act
        var handler = new CreateEventCommandHandler(mockRepository.Object, mockLogger.Object);

        // Assert
        Assert.NotNull(handler);
    }

    [Fact]
    public async Task Handle_ValidEvent_CreatesEventSuccessfully()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = DateTime.UtcNow.AddDays(2);
        var eventDto = new EventDTO
        {
            Name = "Test Event",
            StartDate = startDate,
            EndDate = endDate,
            Location = "Test Location",
            Description = "Test Description",
            RequiredStaffCount = 5
        };
        var command = new CreateEventCommand(eventDto);

        var createdEvent = new Event
        {
            Id = 1,
            Name = eventDto.Name,
            StartDate = eventDto.StartDate,
            EndDate = eventDto.EndDate,
            Location = eventDto.Location,
            Description = eventDto.Description,
            Status = EventStatus.Requested,
            RequiredStaffCount = eventDto.RequiredStaffCount,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Shifts = new List<Shift>
            {
                new Shift
                {
                    Id = 1,
                    Name = "Volledige opdrachtduur",
                    StartTime = eventDto.StartDate,
                    EndTime = eventDto.EndDate,
                    RequiredStaff = eventDto.RequiredStaffCount,
                    Description = "Deelopdracht voor de hele duur van de opdracht",
                    Status = ShiftStatus.Open,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            }
        };

        _mockRepository.Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync(createdEvent);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(eventDto.Name, result.Name);
        _mockRepository.Verify(r => r.CreateEventAsync(It.Is<Event>(e =>
            e.Status == EventStatus.Requested &&
            e.Shifts.Count == 1 &&
            e.Shifts[0].Name == "Volledige opdrachtduur")), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidEvent_CreatesShiftWithCorrectProperties()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = DateTime.UtcNow.AddDays(2);
        var eventDto = new EventDTO
        {
            Name = "Test Event",
            StartDate = startDate,
            EndDate = endDate,
            Location = "Test Location",
            RequiredStaffCount = 3
        };
        var command = new CreateEventCommand(eventDto);

        Event? capturedEvent = null;
        _mockRepository.Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
            .Callback<Event>(e => capturedEvent = e)
            .ReturnsAsync((Event e) => new Event
            {
                Id = 1,
                Name = e.Name,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Location = e.Location,
                Status = e.Status,
                RequiredStaffCount = e.RequiredStaffCount,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                Shifts = e.Shifts
            });

        // Act
        await _handler.Handle(command);

        // Assert
        Assert.NotNull(capturedEvent);
        Assert.Single(capturedEvent.Shifts);
        var shift = capturedEvent.Shifts[0];
        Assert.Equal("Volledige opdrachtduur", shift.Name);
        Assert.Equal(startDate, shift.StartTime);
        Assert.Equal(endDate, shift.EndTime);
        Assert.Equal(3, shift.RequiredStaff);
        Assert.Equal("Deelopdracht voor de hele duur van de opdracht", shift.Description);
        Assert.Equal(ShiftStatus.Open, shift.Status);
    }

    [Fact]
    public async Task Handle_ValidEvent_SetsEventStatusToRequested()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = DateTime.UtcNow.AddDays(2);
        var eventDto = new EventDTO
        {
            Name = "Test Event",
            StartDate = startDate,
            EndDate = endDate,
            Location = "Test Location",
            RequiredStaffCount = 2
        };
        var command = new CreateEventCommand(eventDto);

        Event? capturedEvent = null;
        _mockRepository.Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
            .Callback<Event>(e => capturedEvent = e)
            .ReturnsAsync((Event e) => new Event
            {
                Id = 1,
                Name = e.Name,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Location = e.Location,
                Status = e.Status,
                RequiredStaffCount = e.RequiredStaffCount,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                Shifts = e.Shifts
            });

        // Act
        await _handler.Handle(command);

        // Assert
        Assert.NotNull(capturedEvent);
        Assert.Equal(EventStatus.Requested, capturedEvent.Status);
    }

    [Fact]
    public async Task Handle_ValidEvent_LogsInformation()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = DateTime.UtcNow.AddDays(2);
        var eventDto = new EventDTO
        {
            Name = "Test Event",
            StartDate = startDate,
            EndDate = endDate,
            Location = "Test Location",
            RequiredStaffCount = 2
        };
        var command = new CreateEventCommand(eventDto);

        var createdEvent = new Event
        {
            Id = 123,
            Name = eventDto.Name,
            StartDate = eventDto.StartDate,
            EndDate = eventDto.EndDate,
            Location = eventDto.Location,
            Status = EventStatus.Requested,
            RequiredStaffCount = eventDto.RequiredStaffCount,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Shifts = new List<Shift>()
        };

        _mockRepository.Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync(createdEvent);

        // Act
        await _handler.Handle(command);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("123")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_EndDateBeforeStartDate_ThrowsApplicationLayerException()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(2);
        var endDate = DateTime.UtcNow.AddDays(1);
        var eventDto = new EventDTO
        {
            Name = "Test Event",
            StartDate = startDate,
            EndDate = endDate,
            Location = "Test Location"
        };
        var command = new CreateEventCommand(eventDto);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApplicationLayerException>(() => _handler.Handle(command));
        Assert.Equal("End date must be after start date.", exception.Message);
    }

    [Fact]
    public async Task Handle_EndDateEqualToStartDate_ThrowsApplicationLayerException()
    {
        // Arrange
        var sameDate = DateTime.UtcNow.AddDays(1);
        var eventDto = new EventDTO
        {
            Name = "Test Event",
            StartDate = sameDate,
            EndDate = sameDate,
            Location = "Test Location"
        };
        var command = new CreateEventCommand(eventDto);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApplicationLayerException>(() => _handler.Handle(command));
        Assert.Equal("End date must be after start date.", exception.Message);
    }

    [Fact]
    public async Task Handle_StartDateInPast_ThrowsApplicationLayerException()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-1);
        var endDate = DateTime.UtcNow.AddDays(1);
        var eventDto = new EventDTO
        {
            Name = "Test Event",
            StartDate = startDate,
            EndDate = endDate,
            Location = "Test Location"
        };
        var command = new CreateEventCommand(eventDto);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApplicationLayerException>(() => _handler.Handle(command));
        Assert.Equal("Start date must be in the future.", exception.Message);
    }

    [Fact]
    public async Task Handle_StartDateEqualToNow_ThrowsApplicationLayerException()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var endDate = now.AddDays(1);
        var eventDto = new EventDTO
        {
            Name = "Test Event",
            StartDate = now,
            EndDate = endDate,
            Location = "Test Location"
        };
        var command = new CreateEventCommand(eventDto);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApplicationLayerException>(() => _handler.Handle(command));
        Assert.Equal("Start date must be in the future.", exception.Message);
    }

    [Fact]
    public async Task Handle_ValidEvent_SetsCreatedAtAndUpdatedAt()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = DateTime.UtcNow.AddDays(2);
        var eventDto = new EventDTO
        {
            Name = "Test Event",
            StartDate = startDate,
            EndDate = endDate,
            Location = "Test Location",
            RequiredStaffCount = 2
        };
        var command = new CreateEventCommand(eventDto);

        Event? capturedEvent = null;
        _mockRepository.Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
            .Callback<Event>(e => capturedEvent = e)
            .ReturnsAsync((Event e) => new Event
            {
                Id = 1,
                Name = e.Name,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Location = e.Location,
                Status = e.Status,
                RequiredStaffCount = e.RequiredStaffCount,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                Shifts = e.Shifts
            });

        // Act
        var beforeCall = DateTime.UtcNow;
        await _handler.Handle(command);
        var afterCall = DateTime.UtcNow;

        // Assert
        Assert.NotNull(capturedEvent);
        Assert.True(capturedEvent.CreatedAt >= beforeCall && capturedEvent.CreatedAt <= afterCall);
        Assert.True(capturedEvent.UpdatedAt >= beforeCall && capturedEvent.UpdatedAt <= afterCall);
    }

    [Fact]
    public async Task Handle_WithCancellationToken_PassesToRepository()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = DateTime.UtcNow.AddDays(2);
        var eventDto = new EventDTO
        {
            Name = "Test Event",
            StartDate = startDate,
            EndDate = endDate,
            Location = "Test Location",
            RequiredStaffCount = 2
        };
        var command = new CreateEventCommand(eventDto);
        var cancellationToken = new CancellationToken();

        var createdEvent = new Event
        {
            Id = 1,
            Name = eventDto.Name,
            StartDate = eventDto.StartDate,
            EndDate = eventDto.EndDate,
            Location = eventDto.Location,
            Status = EventStatus.Requested,
            RequiredStaffCount = eventDto.RequiredStaffCount,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Shifts = new List<Shift>()
        };

        _mockRepository.Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync(createdEvent);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.NotNull(result);
        _mockRepository.Verify(r => r.CreateEventAsync(It.IsAny<Event>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidEvent_ReturnsEventDTO()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = DateTime.UtcNow.AddDays(2);
        var eventDto = new EventDTO
        {
            Name = "Test Event",
            StartDate = startDate,
            EndDate = endDate,
            Location = "Test Location",
            Description = "Test Description",
            RequiredStaffCount = 4,
            ContactPerson = "John Doe",
            ContactPhone = "1234567890",
            ContactEmail = "john@example.com",
            CustomerId = 5
        };
        var command = new CreateEventCommand(eventDto);

        var createdEvent = new Event
        {
            Id = 1,
            Name = eventDto.Name,
            StartDate = eventDto.StartDate,
            EndDate = eventDto.EndDate,
            Location = eventDto.Location,
            Description = eventDto.Description,
            Status = EventStatus.Requested,
            RequiredStaffCount = eventDto.RequiredStaffCount,
            ContactPerson = eventDto.ContactPerson,
            ContactPhone = eventDto.ContactPhone,
            ContactEmail = eventDto.ContactEmail,
            CustomerId = eventDto.CustomerId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Shifts = new List<Shift>()
        };

        _mockRepository.Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync(createdEvent);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Event", result.Name);
        Assert.Equal(startDate, result.StartDate);
        Assert.Equal(endDate, result.EndDate);
        Assert.Equal("Test Location", result.Location);
        Assert.Equal("Test Description", result.Description);
        Assert.Equal(4, result.RequiredStaffCount);
        Assert.Equal("John Doe", result.ContactPerson);
        Assert.Equal("1234567890", result.ContactPhone);
        Assert.Equal("john@example.com", result.ContactEmail);
        Assert.Equal(5, result.CustomerId);
    }
}
