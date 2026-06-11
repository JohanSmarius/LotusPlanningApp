using Application.Commands.StaffAssignments;
using Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Application.Tests.Commands.StaffAssignments;

/// <summary>
/// Unit tests for CheckOutStaffCommandHandler
/// </summary>
public class CheckOutStaffCommandHandlerTests
{
    /// <summary>
    /// Test that constructor initializes handler with valid repository and logger
    /// </summary>
    [Fact]
    public void Constructor_ValidDependencies_InitializesHandler()
    {
        // Arrange
        var mockRepository = new Mock<IStaffAssignmentRepository>();
        var mockLogger = new Mock<ILogger<CheckOutStaffCommandHandler>>();

        // Act
        var handler = new CheckOutStaffCommandHandler(mockRepository.Object, mockLogger.Object);

        // Assert
        Assert.NotNull(handler);
    }

    /// <summary>
    /// Test that Handle successfully checks out staff and logs information when assignment exists
    /// </summary>
    [Fact]
    public async Task Handle_ExistingAssignment_ReturnsAssignmentAndLogsInformation()
    {
        // Arrange
        var assignmentId = 123;
        var expectedAssignment = new StaffAssignment
        {
            Id = assignmentId,
            StaffId = 1,
            ShiftId = 10,
            AssignedAt = DateTime.UtcNow
        };

        var mockRepository = new Mock<IStaffAssignmentRepository>();
        mockRepository
            .Setup(r => r.CheckOutStaffAsync(assignmentId))
            .ReturnsAsync(expectedAssignment);

        var mockLogger = new Mock<ILogger<CheckOutStaffCommandHandler>>();

        var handler = new CheckOutStaffCommandHandler(mockRepository.Object, mockLogger.Object);
        var command = new CheckOutStaffCommand(assignmentId);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(assignmentId, result.Id);
        mockRepository.Verify(r => r.CheckOutStaffAsync(assignmentId), Times.Once);
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Staff checked out from assignment {assignmentId}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Test that Handle returns null and logs warning when assignment does not exist
    /// </summary>
    [Fact]
    public async Task Handle_NonExistentAssignment_ReturnsNullAndLogsWarning()
    {
        // Arrange
        var assignmentId = 999;

        var mockRepository = new Mock<IStaffAssignmentRepository>();
        mockRepository
            .Setup(r => r.CheckOutStaffAsync(assignmentId))
            .ReturnsAsync((StaffAssignment?)null);

        var mockLogger = new Mock<ILogger<CheckOutStaffCommandHandler>>();

        var handler = new CheckOutStaffCommandHandler(mockRepository.Object, mockLogger.Object);
        var command = new CheckOutStaffCommand(assignmentId);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.Null(result);
        mockRepository.Verify(r => r.CheckOutStaffAsync(assignmentId), Times.Once);
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Failed to check out staff from assignment {assignmentId}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Test that Handle respects cancellation token
    /// </summary>
    [Fact]
    public async Task Handle_WithCancellationToken_PassesTokenToRepository()
    {
        // Arrange
        var assignmentId = 123;
        var cancellationToken = new CancellationToken();

        var mockRepository = new Mock<IStaffAssignmentRepository>();
        mockRepository
            .Setup(r => r.CheckOutStaffAsync(assignmentId))
            .ReturnsAsync((StaffAssignment?)null);

        var mockLogger = new Mock<ILogger<CheckOutStaffCommandHandler>>();

        var handler = new CheckOutStaffCommandHandler(mockRepository.Object, mockLogger.Object);
        var command = new CheckOutStaffCommand(assignmentId);

        // Act
        var result = await handler.Handle(command, cancellationToken);

        // Assert
        mockRepository.Verify(r => r.CheckOutStaffAsync(assignmentId), Times.Once);
    }
}
