using Application.Commands.Staff;
using Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Application.Tests.Commands.Staff;

/// <summary>
/// Unit tests for DeleteStaffCommandHandler
/// </summary>
public class DeleteStaffCommandHandlerTests
{
    private readonly Mock<IStaffRepository> _mockRepository;
    private readonly Mock<ILogger<DeleteStaffCommandHandler>> _mockLogger;
    private readonly DeleteStaffCommandHandler _handler;

    public DeleteStaffCommandHandlerTests()
    {
        _mockRepository = new Mock<IStaffRepository>();
        _mockLogger = new Mock<ILogger<DeleteStaffCommandHandler>>();
        _handler = new DeleteStaffCommandHandler(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_InitializesHandler()
    {
        // Arrange & Act
        var handler = new DeleteStaffCommandHandler(_mockRepository.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(handler);
    }

    [Fact]
    public async Task Handle_StaffNotFound_ReturnsFalse()
    {
        // Arrange
        var command = new DeleteStaffCommand(1);
        _mockRepository.Setup(r => r.GetStaffByIdAsync(1)).ReturnsAsync((Entities.Staff?)null);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.False(result);
        _mockRepository.Verify(r => r.GetStaffByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.DeleteStaffAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handle_StaffExists_DeletesStaffAndReturnsTrue()
    {
        // Arrange
        var staffMember = new Entities.Staff
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            IsActive = true
        };
        var command = new DeleteStaffCommand(1);
        _mockRepository.Setup(r => r.GetStaffByIdAsync(1)).ReturnsAsync(staffMember);
        _mockRepository.Setup(r => r.DeleteStaffAsync(1)).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.GetStaffByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.DeleteStaffAsync(1), Times.Once);
    }

    [Fact]
    public async Task Handle_StaffNotFound_LogsWarning()
    {
        // Arrange
        var command = new DeleteStaffCommand(1);
        _mockRepository.Setup(r => r.GetStaffByIdAsync(1)).ReturnsAsync((Entities.Staff?)null);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.False(result);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Attempted to delete non-existent staff member")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_StaffExists_LogsInformation()
    {
        // Arrange
        var staffMember = new Entities.Staff
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };
        var command = new DeleteStaffCommand(1);
        _mockRepository.Setup(r => r.GetStaffByIdAsync(1)).ReturnsAsync(staffMember);
        _mockRepository.Setup(r => r.DeleteStaffAsync(1)).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result);
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
    public async Task Handle_WithCancellationToken_DeletesStaff()
    {
        // Arrange
        var staffMember = new Entities.Staff
        {
            Id = 1,
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@example.com"
        };
        var command = new DeleteStaffCommand(1);
        var cancellationToken = new CancellationToken();
        _mockRepository.Setup(r => r.GetStaffByIdAsync(1)).ReturnsAsync(staffMember);
        _mockRepository.Setup(r => r.DeleteStaffAsync(1)).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.GetStaffByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.DeleteStaffAsync(1), Times.Once);
    }

    [Fact]
    public async Task Handle_DifferentStaffIds_DeletesCorrectStaff()
    {
        // Arrange
        var staffId = 42;
        var staffMember = new Entities.Staff
        {
            Id = staffId,
            FirstName = "Alice",
            LastName = "Johnson",
            Email = "alice@example.com"
        };
        var command = new DeleteStaffCommand(staffId);
        _mockRepository.Setup(r => r.GetStaffByIdAsync(staffId)).ReturnsAsync(staffMember);
        _mockRepository.Setup(r => r.DeleteStaffAsync(staffId)).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.GetStaffByIdAsync(staffId), Times.Once);
        _mockRepository.Verify(r => r.DeleteStaffAsync(staffId), Times.Once);
    }
}
