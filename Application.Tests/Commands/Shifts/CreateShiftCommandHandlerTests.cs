using Application.Commands.Shifts;
using Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Application.Tests.Commands.Shifts;

/// <summary>
/// Unit tests for CreateShiftCommandHandler
/// </summary>
public class CreateShiftCommandHandlerTests
{
    private readonly Mock<IShiftRepository> _mockRepository;
    private readonly Mock<ILogger<CreateShiftCommandHandler>> _mockLogger;
    private readonly CreateShiftCommandHandler _handler;

    public CreateShiftCommandHandlerTests()
    {
        _mockRepository = new Mock<IShiftRepository>();
        _mockLogger = new Mock<ILogger<CreateShiftCommandHandler>>();
        _handler = new CreateShiftCommandHandler(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_InitializesHandler()
    {
        // Arrange
        var mockRepository = new Mock<IShiftRepository>();
        var mockLogger = new Mock<ILogger<CreateShiftCommandHandler>>();

        // Act
        var handler = new CreateShiftCommandHandler(mockRepository.Object, mockLogger.Object);

        // Assert
        Assert.NotNull(handler);
    }

    [Fact]
    public async Task Handle_ValidShift_CreatesShiftSuccessfully()
    {
        // Arrange
        var startTime = DateTime.UtcNow.AddHours(1);
        var endTime = DateTime.UtcNow.AddHours(3);
        var shift = new Shift
        {
            EventId = 1,
            Name = "Test Shift",
            StartTime = startTime,
            EndTime = endTime,
            RequiredStaff = 5,
            Description = "Test shift description",
            Status = ShiftStatus.Open
        };
        var command = new CreateShiftCommand(shift);

        var createdShift = new Shift
        {
            Id = 1,
            EventId = shift.EventId,
            Name = shift.Name,
            StartTime = shift.StartTime,
            EndTime = shift.EndTime,
            RequiredStaff = shift.RequiredStaff,
            Description = shift.Description,
            Status = shift.Status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.CreateShiftAsync(It.IsAny<Shift>()))
            .ReturnsAsync(createdShift);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(shift.Name, result.Name);
        Assert.Equal(shift.StartTime, result.StartTime);
        Assert.Equal(shift.EndTime, result.EndTime);
        _mockRepository.Verify(r => r.CreateShiftAsync(It.Is<Shift>(s =>
            s.CreatedAt != default &&
            s.UpdatedAt != default)), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidShift_SetsCreatedAtTimestamp()
    {
        // Arrange
        var startTime = DateTime.UtcNow.AddHours(1);
        var endTime = DateTime.UtcNow.AddHours(3);
        var shift = new Shift
        {
            EventId = 1,
            Name = "Test Shift",
            StartTime = startTime,
            EndTime = endTime,
            RequiredStaff = 3
        };
        var command = new CreateShiftCommand(shift);

        var createdShift = new Shift
        {
            Id = 1,
            EventId = shift.EventId,
            Name = shift.Name,
            StartTime = shift.StartTime,
            EndTime = shift.EndTime,
            RequiredStaff = shift.RequiredStaff,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.CreateShiftAsync(It.IsAny<Shift>()))
            .ReturnsAsync(createdShift);

        var beforeCreation = DateTime.UtcNow.AddSeconds(-1);

        // Act
        await _handler.Handle(command);

        // Assert
        _mockRepository.Verify(r => r.CreateShiftAsync(It.Is<Shift>(s =>
            s.CreatedAt >= beforeCreation &&
            s.CreatedAt <= DateTime.UtcNow.AddSeconds(1))), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidShift_SetsUpdatedAtTimestamp()
    {
        // Arrange
        var startTime = DateTime.UtcNow.AddHours(1);
        var endTime = DateTime.UtcNow.AddHours(3);
        var shift = new Shift
        {
            EventId = 1,
            Name = "Test Shift",
            StartTime = startTime,
            EndTime = endTime,
            RequiredStaff = 3
        };
        var command = new CreateShiftCommand(shift);

        var createdShift = new Shift
        {
            Id = 1,
            EventId = shift.EventId,
            Name = shift.Name,
            StartTime = shift.StartTime,
            EndTime = shift.EndTime,
            RequiredStaff = shift.RequiredStaff,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.CreateShiftAsync(It.IsAny<Shift>()))
            .ReturnsAsync(createdShift);

        var beforeCreation = DateTime.UtcNow.AddSeconds(-1);

        // Act
        await _handler.Handle(command);

        // Assert
        _mockRepository.Verify(r => r.CreateShiftAsync(It.Is<Shift>(s =>
            s.UpdatedAt != null &&
            s.UpdatedAt >= beforeCreation &&
            s.UpdatedAt <= DateTime.UtcNow.AddSeconds(1))), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidShift_LogsInformation()
    {
        // Arrange
        var startTime = DateTime.UtcNow.AddHours(1);
        var endTime = DateTime.UtcNow.AddHours(3);
        var shift = new Shift
        {
            EventId = 1,
            Name = "Test Shift",
            StartTime = startTime,
            EndTime = endTime,
            RequiredStaff = 3
        };
        var command = new CreateShiftCommand(shift);

        var createdShift = new Shift
        {
            Id = 42,
            EventId = shift.EventId,
            Name = shift.Name,
            StartTime = shift.StartTime,
            EndTime = shift.EndTime,
            RequiredStaff = shift.RequiredStaff,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.CreateShiftAsync(It.IsAny<Shift>()))
            .ReturnsAsync(createdShift);

        // Act
        await _handler.Handle(command);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("42") && v.ToString()!.Contains("created successfully")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_StartTimeEqualsEndTime_ThrowsApplicationLayerException()
    {
        // Arrange
        var time = DateTime.UtcNow.AddHours(1);
        var shift = new Shift
        {
            EventId = 1,
            Name = "Test Shift",
            StartTime = time,
            EndTime = time,
            RequiredStaff = 3
        };
        var command = new CreateShiftCommand(shift);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApplicationLayerException>(
            () => _handler.Handle(command));
        Assert.Equal("Shift end time must be after start time.", exception.Message);
        _mockRepository.Verify(r => r.CreateShiftAsync(It.IsAny<Shift>()), Times.Never);
    }

    [Fact]
    public async Task Handle_StartTimeAfterEndTime_ThrowsApplicationLayerException()
    {
        // Arrange
        var startTime = DateTime.UtcNow.AddHours(3);
        var endTime = DateTime.UtcNow.AddHours(1);
        var shift = new Shift
        {
            EventId = 1,
            Name = "Test Shift",
            StartTime = startTime,
            EndTime = endTime,
            RequiredStaff = 3
        };
        var command = new CreateShiftCommand(shift);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApplicationLayerException>(
            () => _handler.Handle(command));
        Assert.Equal("Shift end time must be after start time.", exception.Message);
        _mockRepository.Verify(r => r.CreateShiftAsync(It.IsAny<Shift>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidShift_CallsRepositoryCreateShiftAsync()
    {
        // Arrange
        var startTime = DateTime.UtcNow.AddHours(1);
        var endTime = DateTime.UtcNow.AddHours(3);
        var shift = new Shift
        {
            EventId = 1,
            Name = "Test Shift",
            StartTime = startTime,
            EndTime = endTime,
            RequiredStaff = 3
        };
        var command = new CreateShiftCommand(shift);

        var createdShift = new Shift
        {
            Id = 1,
            EventId = shift.EventId,
            Name = shift.Name,
            StartTime = shift.StartTime,
            EndTime = shift.EndTime,
            RequiredStaff = shift.RequiredStaff,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.CreateShiftAsync(It.IsAny<Shift>()))
            .ReturnsAsync(createdShift);

        // Act
        await _handler.Handle(command);

        // Assert
        _mockRepository.Verify(r => r.CreateShiftAsync(shift), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidShift_ReturnsCreatedShift()
    {
        // Arrange
        var startTime = DateTime.UtcNow.AddHours(1);
        var endTime = DateTime.UtcNow.AddHours(3);
        var shift = new Shift
        {
            EventId = 1,
            Name = "Test Shift",
            StartTime = startTime,
            EndTime = endTime,
            RequiredStaff = 5,
            Description = "Test Description"
        };
        var command = new CreateShiftCommand(shift);

        var createdShift = new Shift
        {
            Id = 99,
            EventId = shift.EventId,
            Name = shift.Name,
            StartTime = shift.StartTime,
            EndTime = shift.EndTime,
            RequiredStaff = shift.RequiredStaff,
            Description = shift.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.CreateShiftAsync(It.IsAny<Shift>()))
            .ReturnsAsync(createdShift);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.Same(createdShift, result);
    }

    [Fact]
    public async Task Handle_ValidShiftWithCancellationToken_CreatesShiftSuccessfully()
    {
        // Arrange
        var startTime = DateTime.UtcNow.AddHours(1);
        var endTime = DateTime.UtcNow.AddHours(3);
        var shift = new Shift
        {
            EventId = 1,
            Name = "Test Shift",
            StartTime = startTime,
            EndTime = endTime,
            RequiredStaff = 3
        };
        var command = new CreateShiftCommand(shift);
        var cancellationToken = new CancellationToken();

        var createdShift = new Shift
        {
            Id = 1,
            EventId = shift.EventId,
            Name = shift.Name,
            StartTime = shift.StartTime,
            EndTime = shift.EndTime,
            RequiredStaff = shift.RequiredStaff,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.CreateShiftAsync(It.IsAny<Shift>()))
            .ReturnsAsync(createdShift);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }
}
