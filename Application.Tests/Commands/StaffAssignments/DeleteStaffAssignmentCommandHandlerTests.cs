using Application.Commands.StaffAssignments;
using Moq;
using Xunit;

namespace Application.Tests.Commands.StaffAssignments;

/// <summary>
/// Unit tests for DeleteStaffAssignmentCommandHandler
/// </summary>
public class DeleteStaffAssignmentCommandHandlerTests
{
    /// <summary>
    /// Test that constructor initializes handler with valid repository
    /// </summary>
    [Fact]
    public void Constructor_ValidRepository_InitializesHandler()
    {
        // Arrange
        var mockRepository = new Mock<IStaffAssignmentRepository>();

        // Act
        var handler = new DeleteStaffAssignmentCommandHandler(mockRepository.Object);

        // Assert
        Assert.NotNull(handler);
    }

    /// <summary>
    /// Test that Handle calls repository DeleteAssignmentAsync with correct assignment ID
    /// </summary>
    [Fact]
    public async Task Handle_ValidCommand_CallsDeleteAssignmentAsync()
    {
        // Arrange
        var assignmentId = 123;
        var mockRepository = new Mock<IStaffAssignmentRepository>();
        mockRepository
            .Setup(r => r.DeleteAssignmentAsync(assignmentId))
            .Returns(Task.CompletedTask);

        var handler = new DeleteStaffAssignmentCommandHandler(mockRepository.Object);
        var command = new DeleteStaffAssignmentCommand(assignmentId);

        // Act
        var result = await handler.Handle(command);

        // Assert
        mockRepository.Verify(r => r.DeleteAssignmentAsync(assignmentId), Times.Once);
    }

    /// <summary>
    /// Test that Handle returns true on successful deletion
    /// </summary>
    [Fact]
    public async Task Handle_ValidCommand_ReturnsTrue()
    {
        // Arrange
        var assignmentId = 456;
        var mockRepository = new Mock<IStaffAssignmentRepository>();
        mockRepository
            .Setup(r => r.DeleteAssignmentAsync(assignmentId))
            .Returns(Task.CompletedTask);

        var handler = new DeleteStaffAssignmentCommandHandler(mockRepository.Object);
        var command = new DeleteStaffAssignmentCommand(assignmentId);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Test that Handle respects cancellation token
    /// </summary>
    [Fact]
    public async Task Handle_WithCancellationToken_PassesTokenToRepository()
    {
        // Arrange
        var assignmentId = 789;
        var cancellationToken = new CancellationToken();
        var mockRepository = new Mock<IStaffAssignmentRepository>();
        mockRepository
            .Setup(r => r.DeleteAssignmentAsync(assignmentId))
            .Returns(Task.CompletedTask);

        var handler = new DeleteStaffAssignmentCommandHandler(mockRepository.Object);
        var command = new DeleteStaffAssignmentCommand(assignmentId);

        // Act
        var result = await handler.Handle(command, cancellationToken);

        // Assert
        Assert.True(result);
        mockRepository.Verify(r => r.DeleteAssignmentAsync(assignmentId), Times.Once);
    }

    /// <summary>
    /// Test that Handle works with zero assignment ID
    /// </summary>
    [Fact]
    public async Task Handle_ZeroAssignmentId_CallsDeleteAssignmentAsync()
    {
        // Arrange
        var assignmentId = 0;
        var mockRepository = new Mock<IStaffAssignmentRepository>();
        mockRepository
            .Setup(r => r.DeleteAssignmentAsync(assignmentId))
            .Returns(Task.CompletedTask);

        var handler = new DeleteStaffAssignmentCommandHandler(mockRepository.Object);
        var command = new DeleteStaffAssignmentCommand(assignmentId);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result);
        mockRepository.Verify(r => r.DeleteAssignmentAsync(assignmentId), Times.Once);
    }

    /// <summary>
    /// Test that Handle works with negative assignment ID
    /// </summary>
    [Fact]
    public async Task Handle_NegativeAssignmentId_CallsDeleteAssignmentAsync()
    {
        // Arrange
        var assignmentId = -1;
        var mockRepository = new Mock<IStaffAssignmentRepository>();
        mockRepository
            .Setup(r => r.DeleteAssignmentAsync(assignmentId))
            .Returns(Task.CompletedTask);

        var handler = new DeleteStaffAssignmentCommandHandler(mockRepository.Object);
        var command = new DeleteStaffAssignmentCommand(assignmentId);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result);
        mockRepository.Verify(r => r.DeleteAssignmentAsync(assignmentId), Times.Once);
    }
}
