using Application.Commands.Shifts;
using Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Application.Tests.Commands.Shifts;

/// <summary>
/// Tests for DeleteShiftCommandHandler
/// </summary>
public class DeleteShiftCommandHandlerTests
{
    private readonly Mock<IShiftRepository> _mockRepository;
    private readonly Mock<ILogger<DeleteShiftCommandHandler>> _mockLogger;
    private readonly DeleteShiftCommandHandler _handler;

    public DeleteShiftCommandHandlerTests()
    {
        _mockRepository = new Mock<IShiftRepository>();
        _mockLogger = new Mock<ILogger<DeleteShiftCommandHandler>>();
        _handler = new DeleteShiftCommandHandler(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_ValidParameters_InitializesHandler()
    {
        // Arrange & Act
        var handler = new DeleteShiftCommandHandler(_mockRepository.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(handler);
    }

    [Fact]
    public async Task Handle_ShiftNotFound_ReturnsFalse()
    {
        // Arrange
        var command = new DeleteShiftCommand(1);
        _mockRepository.Setup(r => r.GetShiftByIdAsync(1)).ReturnsAsync((Shift?)null);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.False(result);
        _mockRepository.Verify(r => r.GetShiftByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.DeleteShiftAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidDelete_DeletesShiftAndReturnsTrue()
    {
        // Arrange
        var shift = new Shift
        {
            Id = 1,
            EventId = 1,
            Name = "Morning Shift",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(8),
            RequiredStaff = 3
        };
        var command = new DeleteShiftCommand(1);
        _mockRepository.Setup(r => r.GetShiftByIdAsync(1)).ReturnsAsync(shift);
        _mockRepository.Setup(r => r.DeleteShiftAsync(1)).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.GetShiftByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.DeleteShiftAsync(1), Times.Once);
    }

    [Fact]
    public async Task Handle_ShiftNotFound_LogsWarning()
    {
        // Arrange
        var command = new DeleteShiftCommand(999);
        _mockRepository.Setup(r => r.GetShiftByIdAsync(999)).ReturnsAsync((Shift?)null);

        // Act
        await _handler.Handle(command);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Attempted to delete non-existent shift")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidDelete_LogsInformation()
    {
        // Arrange
        var shift = new Shift
        {
            Id = 2,
            EventId = 1,
            Name = "Evening Shift",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(8),
            RequiredStaff = 2
        };
        var command = new DeleteShiftCommand(2);
        _mockRepository.Setup(r => r.GetShiftByIdAsync(2)).ReturnsAsync(shift);
        _mockRepository.Setup(r => r.DeleteShiftAsync(2)).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("deleted successfully")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithCancellationToken_DeletesShift()
    {
        // Arrange
        var shift = new Shift
        {
            Id = 3,
            EventId = 1,
            Name = "Night Shift",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(8),
            RequiredStaff = 1
        };
        var command = new DeleteShiftCommand(3);
        var cancellationToken = new CancellationToken();
        _mockRepository.Setup(r => r.GetShiftByIdAsync(3)).ReturnsAsync(shift);
        _mockRepository.Setup(r => r.DeleteShiftAsync(3)).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteShiftAsync(3), Times.Once);
    }
}
