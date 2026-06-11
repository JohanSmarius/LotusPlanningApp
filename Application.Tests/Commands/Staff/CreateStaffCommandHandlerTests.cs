using Application.Commands.Staff;
using Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Application.Tests.Commands.Staff;

/// <summary>
/// Unit tests for CreateStaffCommandHandler
/// </summary>
public class CreateStaffCommandHandlerTests
{
    private readonly Mock<IStaffRepository> _mockRepository;
    private readonly Mock<ILogger<CreateStaffCommandHandler>> _mockLogger;
    private readonly CreateStaffCommandHandler _handler;

    public CreateStaffCommandHandlerTests()
    {
        _mockRepository = new Mock<IStaffRepository>();
        _mockLogger = new Mock<ILogger<CreateStaffCommandHandler>>();
        _handler = new CreateStaffCommandHandler(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_InitializesHandler()
    {
        // Arrange
        var mockRepository = new Mock<IStaffRepository>();
        var mockLogger = new Mock<ILogger<CreateStaffCommandHandler>>();

        // Act
        var handler = new CreateStaffCommandHandler(mockRepository.Object, mockLogger.Object);

        // Assert
        Assert.NotNull(handler);
    }

    [Fact]
    public async Task Handle_WithUniqueEmail_CreatesStaffSuccessfully()
    {
        // Arrange
        var staff = new Entities.Staff
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Phone = "1234567890",
            CertificationLevel = "Advanced",
            IsActive = true
        };
        var command = new CreateStaffCommand(staff);

        var createdStaff = new Entities.Staff
        {
            Id = 1,
            FirstName = staff.FirstName,
            LastName = staff.LastName,
            Email = staff.Email,
            Phone = staff.Phone,
            CertificationLevel = staff.CertificationLevel,
            IsActive = staff.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.IsEmailUniqueAsync(staff.Email, null))
            .ReturnsAsync(true);
        _mockRepository.Setup(r => r.CreateStaffAsync(staff))
            .ReturnsAsync(createdStaff);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(staff.FirstName, result.FirstName);
        Assert.Equal(staff.LastName, result.LastName);
        Assert.Equal(staff.Email, result.Email);
        _mockRepository.Verify(r => r.IsEmailUniqueAsync(staff.Email, null), Times.Once);
        _mockRepository.Verify(r => r.CreateStaffAsync(staff), Times.Once);
    }

    [Fact]
    public async Task Handle_WithUniqueEmail_SetsCreatedAtTimestamp()
    {
        // Arrange
        var beforeTest = DateTime.UtcNow.AddSeconds(-1);
        var staff = new Entities.Staff
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com"
        };
        var command = new CreateStaffCommand(staff);

        var createdStaff = new Entities.Staff
        {
            Id = 2,
            FirstName = staff.FirstName,
            LastName = staff.LastName,
            Email = staff.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.IsEmailUniqueAsync(staff.Email, null))
            .ReturnsAsync(true);
        _mockRepository.Setup(r => r.CreateStaffAsync(staff))
            .ReturnsAsync(createdStaff);

        // Act
        await _handler.Handle(command);
        var afterTest = DateTime.UtcNow.AddSeconds(1);

        // Assert
        Assert.True(staff.CreatedAt >= beforeTest && staff.CreatedAt <= afterTest);
        Assert.True(staff.UpdatedAt >= beforeTest && staff.UpdatedAt <= afterTest);
    }

    [Fact]
    public async Task Handle_WithUniqueEmail_SetsUpdatedAtTimestamp()
    {
        // Arrange
        var beforeTest = DateTime.UtcNow.AddSeconds(-1);
        var staff = new Entities.Staff
        {
            FirstName = "Bob",
            LastName = "Johnson",
            Email = "bob.johnson@example.com"
        };
        var command = new CreateStaffCommand(staff);

        var createdStaff = new Entities.Staff
        {
            Id = 3,
            FirstName = staff.FirstName,
            LastName = staff.LastName,
            Email = staff.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.IsEmailUniqueAsync(staff.Email, null))
            .ReturnsAsync(true);
        _mockRepository.Setup(r => r.CreateStaffAsync(staff))
            .ReturnsAsync(createdStaff);

        // Act
        await _handler.Handle(command);
        var afterTest = DateTime.UtcNow.AddSeconds(1);

        // Assert
        Assert.NotNull(staff.UpdatedAt);
        Assert.True(staff.UpdatedAt >= beforeTest && staff.UpdatedAt <= afterTest);
    }

    [Fact]
    public async Task Handle_WithDuplicateEmail_ThrowsApplicationLayerException()
    {
        // Arrange
        var staff = new Entities.Staff
        {
            FirstName = "Alice",
            LastName = "Brown",
            Email = "duplicate@example.com"
        };
        var command = new CreateStaffCommand(staff);

        _mockRepository.Setup(r => r.IsEmailUniqueAsync(staff.Email, null))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApplicationLayerException>(
            () => _handler.Handle(command));
        Assert.Equal($"Email {staff.Email} is already in use.", exception.Message);
        _mockRepository.Verify(r => r.IsEmailUniqueAsync(staff.Email, null), Times.Once);
        _mockRepository.Verify(r => r.CreateStaffAsync(It.IsAny<Entities.Staff>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithUniqueEmail_LogsSuccessMessage()
    {
        // Arrange
        var staff = new Entities.Staff
        {
            FirstName = "Charlie",
            LastName = "Wilson",
            Email = "charlie.wilson@example.com"
        };
        var command = new CreateStaffCommand(staff);

        var createdStaff = new Entities.Staff
        {
            Id = 42,
            FirstName = staff.FirstName,
            LastName = staff.LastName,
            Email = staff.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.IsEmailUniqueAsync(staff.Email, null))
            .ReturnsAsync(true);
        _mockRepository.Setup(r => r.CreateStaffAsync(staff))
            .ReturnsAsync(createdStaff);

        // Act
        await _handler.Handle(command);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Staff member 42 created successfully.")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithCancellationToken_CreatesStaffSuccessfully()
    {
        // Arrange
        var staff = new Entities.Staff
        {
            FirstName = "David",
            LastName = "Martinez",
            Email = "david.martinez@example.com"
        };
        var command = new CreateStaffCommand(staff);
        var cancellationToken = new CancellationToken();

        var createdStaff = new Entities.Staff
        {
            Id = 5,
            FirstName = staff.FirstName,
            LastName = staff.LastName,
            Email = staff.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.IsEmailUniqueAsync(staff.Email, null))
            .ReturnsAsync(true);
        _mockRepository.Setup(r => r.CreateStaffAsync(staff))
            .ReturnsAsync(createdStaff);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Id);
    }

    [Fact]
    public async Task Handle_WithAllStaffProperties_PreservesAllData()
    {
        // Arrange
        var staff = new Entities.Staff
        {
            FirstName = "Emma",
            LastName = "Taylor",
            Email = "emma.taylor@example.com",
            Phone = "9876543210",
            CertificationLevel = "Expert",
            CertificationExpiry = DateTime.UtcNow.AddYears(1),
            IsActive = true
        };
        var command = new CreateStaffCommand(staff);

        var createdStaff = new Entities.Staff
        {
            Id = 6,
            FirstName = staff.FirstName,
            LastName = staff.LastName,
            Email = staff.Email,
            Phone = staff.Phone,
            CertificationLevel = staff.CertificationLevel,
            CertificationExpiry = staff.CertificationExpiry,
            IsActive = staff.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.IsEmailUniqueAsync(staff.Email, null))
            .ReturnsAsync(true);
        _mockRepository.Setup(r => r.CreateStaffAsync(staff))
            .ReturnsAsync(createdStaff);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(staff.Phone, result.Phone);
        Assert.Equal(staff.CertificationLevel, result.CertificationLevel);
        Assert.Equal(staff.CertificationExpiry, result.CertificationExpiry);
        Assert.Equal(staff.IsActive, result.IsActive);
    }

    [Fact]
    public async Task Handle_WithUniqueEmail_ReturnsCreatedStaffFromRepository()
    {
        // Arrange
        var staff = new Entities.Staff
        {
            FirstName = "Frank",
            LastName = "Anderson",
            Email = "frank.anderson@example.com"
        };
        var command = new CreateStaffCommand(staff);

        var createdStaff = new Entities.Staff
        {
            Id = 7,
            FirstName = staff.FirstName,
            LastName = staff.LastName,
            Email = staff.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.IsEmailUniqueAsync(staff.Email, null))
            .ReturnsAsync(true);
        _mockRepository.Setup(r => r.CreateStaffAsync(staff))
            .ReturnsAsync(createdStaff);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.Same(createdStaff, result);
    }
}
