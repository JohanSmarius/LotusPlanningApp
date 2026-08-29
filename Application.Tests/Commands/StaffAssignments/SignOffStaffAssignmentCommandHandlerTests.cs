using Application.Commands.StaffAssignments;
using Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Application.Tests.Commands.StaffAssignments;

public class SignOffStaffAssignmentCommandHandlerTests
{
    private readonly Mock<IStaffAssignmentRepository> _mockRepository;
    private readonly Mock<ILogger<SignOffStaffAssignmentCommandHandler>> _mockLogger;
    private readonly SignOffStaffAssignmentCommandHandler _handler;

    public SignOffStaffAssignmentCommandHandlerTests()
    {
        _mockRepository = new Mock<IStaffAssignmentRepository>();
        _mockLogger = new Mock<ILogger<SignOffStaffAssignmentCommandHandler>>();
        _handler = new SignOffStaffAssignmentCommandHandler(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ValidSignOff_ReturnsSignedOffAssignment()
    {
        // Arrange
        var signature = new byte[] { 1, 2, 3 };
        var command = new SignOffStaffAssignmentCommand(1, 4.5m, 12m, signature);
        var assignment = new StaffAssignment
        {
            Id = 1,
            ActualHours = 4.5m,
            KilometersDriven = 12m,
            ClientSignature = signature,
            SignedOffAt = DateTime.UtcNow
        };
        _mockRepository
            .Setup(r => r.SignOffAssignmentAsync(1, 4.5m, 12m, signature))
            .ReturnsAsync(assignment);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(assignment.Id, result!.Id);
        _mockRepository.Verify(r => r.SignOffAssignmentAsync(1, 4.5m, 12m, signature), Times.Once);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(4, -1)]
    public async Task Handle_InvalidHoursOrKilometers_ReturnsNullWithoutCallingRepository(double hours, double kilometers)
    {
        // Arrange
        var command = new SignOffStaffAssignmentCommand(1, (decimal)hours, (decimal)kilometers, new byte[] { 1 });

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(
            r => r.SignOffAssignmentAsync(It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<byte[]>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_EmptySignature_ReturnsNullWithoutCallingRepository()
    {
        // Arrange
        var command = new SignOffStaffAssignmentCommand(1, 4m, 10m, Array.Empty<byte>());

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(
            r => r.SignOffAssignmentAsync(It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<byte[]>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_AssignmentAlreadySignedOffOrNotFound_ReturnsNull()
    {
        // Arrange
        var signature = new byte[] { 1, 2, 3 };
        var command = new SignOffStaffAssignmentCommand(1, 4m, 10m, signature);
        _mockRepository
            .Setup(r => r.SignOffAssignmentAsync(1, 4m, 10m, signature))
            .ReturnsAsync((StaffAssignment?)null);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.SignOffAssignmentAsync(1, 4m, 10m, signature), Times.Once);
    }
}
