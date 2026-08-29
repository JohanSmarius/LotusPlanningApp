using Application;
using Application.Commands.StaffAssignments;
using Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Application.Tests.Commands.StaffAssignments;

/// <summary>
/// Tests that verify an email notification is sent when a staff assignment is deleted.
/// </summary>
public class DeleteStaffAssignmentCommandHandlerTests
{
    private readonly Mock<IStaffAssignmentRepository> _mockRepository;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<ILogger<DeleteStaffAssignmentCommandHandler>> _mockLogger;
    private readonly DeleteStaffAssignmentCommandHandler _handler;

    public DeleteStaffAssignmentCommandHandlerTests()
    {
        _mockRepository = new Mock<IStaffAssignmentRepository>();
        _mockEmailService = new Mock<IEmailService>();
        _mockLogger = new Mock<ILogger<DeleteStaffAssignmentCommandHandler>>();
        _handler = new DeleteStaffAssignmentCommandHandler(
            _mockRepository.Object, _mockEmailService.Object, _mockLogger.Object);
    }

    private static StaffAssignment CreateAssignment()
    {
        var staff = new Staff
        {
            Id = 1,
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var evt = new Event
        {
            Id = 1,
            Name = "Test Event",
            Location = "Venue A",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            CreatedAt = DateTime.UtcNow
        };

        var shift = new Shift
        {
            Id = 1,
            Name = "Morning Shift",
            StartTime = evt.StartDate,
            EndTime = evt.StartDate.AddHours(8),
            EventId = evt.Id,
            Event = evt,
            CreatedAt = DateTime.UtcNow
        };

        return new StaffAssignment
        {
            Id = 1,
            StaffId = staff.Id,
            Staff = staff,
            ShiftId = shift.Id,
            Shift = shift,
            Status = AssignmentStatus.Assigned,
            AssignedAt = DateTime.UtcNow
        };
    }

    [Fact]
    public async Task Handle_WhenAssignmentExists_SendsDeletionEmail()
    {
        // Arrange
        var assignment = CreateAssignment();
        _mockRepository.Setup(r => r.GetAssignmentByIdAsync(assignment.Id)).ReturnsAsync(assignment);
        var command = new DeleteStaffAssignmentCommand(assignment.Id);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteAssignmentAsync(assignment.Id), Times.Once);
        _mockEmailService.Verify(
            s => s.SendStaffAssignmentDeletionNotificationAsync(
                It.Is<Staff>(st => st.Id == assignment.Staff!.Id),
                It.Is<Shift>(sh => sh.Id == assignment.Shift!.Id),
                It.Is<Event>(e => e.Id == assignment.Shift!.Event!.Id)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAssignmentDoesNotExist_DoesNotSendEmail()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAssignmentByIdAsync(It.IsAny<int>())).ReturnsAsync((StaffAssignment?)null);
        var command = new DeleteStaffAssignmentCommand(99999);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteAssignmentAsync(command.AssignmentId), Times.Once);
        _mockEmailService.Verify(
            s => s.SendStaffAssignmentDeletionNotificationAsync(
                It.IsAny<Staff>(), It.IsAny<Shift>(), It.IsAny<Event>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenEmailFails_AssignmentDeletionStillSucceeds()
    {
        // Arrange
        var assignment = CreateAssignment();
        _mockRepository.Setup(r => r.GetAssignmentByIdAsync(assignment.Id)).ReturnsAsync(assignment);
        _mockEmailService
            .Setup(s => s.SendStaffAssignmentDeletionNotificationAsync(
                It.IsAny<Staff>(), It.IsAny<Shift>(), It.IsAny<Event>()))
            .ThrowsAsync(new InvalidOperationException("SMTP error"));
        var command = new DeleteStaffAssignmentCommand(assignment.Id);

        // Act
        var result = await _handler.Handle(command);

        // Assert: handler does not throw and still reports success.
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteAssignmentAsync(assignment.Id), Times.Once);
    }
}
