using Application;
using Application.Commands.Staff;
using Application.Common;
using Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Application.Tests.Commands.Staff;

/// <summary>
/// Tests for UpdateStaffCommandHandler
/// </summary>
public class UpdateStaffCommandHandlerTests
{
    private readonly Mock<IStaffRepository> _mockRepository;
    private readonly Mock<ILogger<UpdateStaffCommandHandler>> _mockLogger;
    private readonly UpdateStaffCommandHandler _handler;

    public UpdateStaffCommandHandlerTests()
    {
        _mockRepository = new Mock<IStaffRepository>();
        _mockLogger = new Mock<ILogger<UpdateStaffCommandHandler>>();
        _handler = new UpdateStaffCommandHandler(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_InitializesHandler()
    {
        // Arrange & Act
        var handler = new UpdateStaffCommandHandler(_mockRepository.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(handler);
    }

    [Fact]
    public async Task Handle_EmailNotUnique_ThrowsApplicationLayerException()
    {
        // Arrange
        var staff = new Entities.Staff
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };
        var command = new UpdateStaffCommand(staff);
        _mockRepository.Setup(r => r.IsEmailUniqueAsync(staff.Email, staff.Id)).ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApplicationLayerException>(() => _handler.Handle(command));
        Assert.Contains("already in use", exception.Message);
        Assert.Contains(staff.Email, exception.Message);
    }

    [Fact]
    public async Task Handle_ValidUpdate_UpdatesStaffAndReturnsResult()
    {
        // Arrange
        var staff = new Entities.Staff
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };
        var command = new UpdateStaffCommand(staff);
        var updatedStaff = new Entities.Staff
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.IsEmailUniqueAsync(staff.Email, staff.Id)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.UpdateStaffAsync(staff)).ReturnsAsync(updatedStaff);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedStaff.Id, result.Id);
        _mockRepository.Verify(r => r.UpdateStaffAsync(staff), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidUpdate_SetsUpdatedAt()
    {
        // Arrange
        var beforeUpdate = DateTime.UtcNow;
        var staff = new Entities.Staff
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            UpdatedAt = null
        };
        var command = new UpdateStaffCommand(staff);

        _mockRepository.Setup(r => r.IsEmailUniqueAsync(staff.Email, staff.Id)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.UpdateStaffAsync(staff)).ReturnsAsync(staff);

        // Act
        await _handler.Handle(command);
        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.NotNull(staff.UpdatedAt);
        Assert.True(staff.UpdatedAt >= beforeUpdate);
        Assert.True(staff.UpdatedAt <= afterUpdate);
    }

    [Fact]
    public async Task Handle_ValidUpdate_LogsInformation()
    {
        // Arrange
        var staff = new Entities.Staff
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };
        var command = new UpdateStaffCommand(staff);

        _mockRepository.Setup(r => r.IsEmailUniqueAsync(staff.Email, staff.Id)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.UpdateStaffAsync(staff)).ReturnsAsync(staff);

        // Act
        await _handler.Handle(command);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("updated successfully")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithCancellationToken_UpdatesStaffSuccessfully()
    {
        // Arrange
        var staff = new Entities.Staff
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };
        var command = new UpdateStaffCommand(staff);
        var cancellationToken = new CancellationToken();

        _mockRepository.Setup(r => r.IsEmailUniqueAsync(staff.Email, staff.Id)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.UpdateStaffAsync(staff)).ReturnsAsync(staff);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(staff.Id, result.Id);
    }

    [Fact]
    public async Task Handle_EmailUniqueCheck_PassesCorrectParameters()
    {
        // Arrange
        var staff = new Entities.Staff
        {
            Id = 42,
            FirstName = "John",
            LastName = "Doe",
            Email = "unique@example.com"
        };
        var command = new UpdateStaffCommand(staff);

        _mockRepository.Setup(r => r.IsEmailUniqueAsync(staff.Email, staff.Id)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.UpdateStaffAsync(staff)).ReturnsAsync(staff);

        // Act
        await _handler.Handle(command);

        // Assert
        _mockRepository.Verify(r => r.IsEmailUniqueAsync("unique@example.com", 42), Times.Once);
    }
}
