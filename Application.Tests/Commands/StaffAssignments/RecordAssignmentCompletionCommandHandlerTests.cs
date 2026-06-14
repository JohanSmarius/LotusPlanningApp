using Application.Commands.StaffAssignments;
using Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Application.Tests.Commands.StaffAssignments;

public class RecordAssignmentCompletionCommandHandlerTests
{
    private readonly Mock<IStaffAssignmentRepository> _mockRepository;
    private readonly Mock<ILogger<RecordAssignmentCompletionCommandHandler>> _mockLogger;
    private readonly RecordAssignmentCompletionCommandHandler _handler;

    public RecordAssignmentCompletionCommandHandlerTests()
    {
        _mockRepository = new Mock<IStaffAssignmentRepository>();
        _mockLogger = new Mock<ILogger<RecordAssignmentCompletionCommandHandler>>();
        _handler = new RecordAssignmentCompletionCommandHandler(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_AssignmentNotFound_ReturnsNull()
    {
        // Arrange
        var command = new RecordAssignmentCompletionCommand(99, 42, null, null);
        _mockRepository
            .Setup(r => r.RecordAssignmentCompletionAsync(99, 42, null, null))
            .ReturnsAsync((StaffAssignment?)null);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.RecordAssignmentCompletionAsync(99, 42, null, null), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsUpdatedAssignment()
    {
        // Arrange
        var assignment = new StaffAssignment
        {
            Id = 1,
            ShiftId = 10,
            StaffId = 5,
            Role = StaffRole.LOTUS,
            Status = AssignmentStatus.CheckedOut,
            KmDriven = 30,
            CustomerSignedName = "Jan Janssen",
            CustomerSignature = "data:image/png;base64,abc123",
            CustomerSignedAt = DateTime.UtcNow
        };

        var command = new RecordAssignmentCompletionCommand(1, 30, "data:image/png;base64,abc123", "Jan Janssen");

        _mockRepository
            .Setup(r => r.RecordAssignmentCompletionAsync(1, 30, "data:image/png;base64,abc123", "Jan Janssen"))
            .ReturnsAsync(assignment);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(30, result.KmDriven);
        Assert.Equal("Jan Janssen", result.CustomerSignedName);
        Assert.Equal("data:image/png;base64,abc123", result.CustomerSignature);
        _mockRepository.Verify(r => r.RecordAssignmentCompletionAsync(1, 30, "data:image/png;base64,abc123", "Jan Janssen"), Times.Once);
    }

    [Fact]
    public async Task Handle_KmDrivenOnly_NoSignature_Succeeds()
    {
        // Arrange
        var assignment = new StaffAssignment
        {
            Id = 2,
            ShiftId = 10,
            StaffId = 5,
            Role = StaffRole.LOTUS,
            Status = AssignmentStatus.CheckedOut,
            KmDriven = 15
        };

        var command = new RecordAssignmentCompletionCommand(2, 15, null, null);

        _mockRepository
            .Setup(r => r.RecordAssignmentCompletionAsync(2, 15, null, null))
            .ReturnsAsync(assignment);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(15, result.KmDriven);
        Assert.Null(result.CustomerSignature);
        Assert.Null(result.CustomerSignedName);
    }
}
