using Application;
using Application.Commands.Shifts;
using Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Application.Tests.Commands.Shifts;

/// <summary>
/// Tests that verify an email notification is sent to assigned staff when a shift is deleted.
/// </summary>
public class DeleteShiftCommandHandlerTests
{
    private readonly Mock<IShiftRepository> _mockRepository;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<ILogger<DeleteShiftCommandHandler>> _mockLogger;
    private readonly DeleteShiftCommandHandler _handler;

    public DeleteShiftCommandHandlerTests()
    {
        _mockRepository = new Mock<IShiftRepository>();
        _mockEmailService = new Mock<IEmailService>();
        _mockLogger = new Mock<ILogger<DeleteShiftCommandHandler>>();
        _handler = new DeleteShiftCommandHandler(
            _mockRepository.Object, _mockEmailService.Object, _mockLogger.Object);
    }

    private static (Shift Shift, Staff Staff1, Staff Staff2) CreateShiftWithStaff()
    {
        var staff1 = new Staff
        {
            Id = 1,
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var staff2 = new Staff
        {
            Id = 2,
            FirstName = "John",
            LastName = "Smith",
            Email = "john.smith@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var evt = new Event
        {
            Id = 1,
            Name = "Festival Event",
            Location = "Main Square",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            CreatedAt = DateTime.UtcNow
        };

        var shift = new Shift
        {
            Id = 10,
            Name = "Day Shift",
            StartTime = evt.StartDate,
            EndTime = evt.StartDate.AddHours(8),
            EventId = evt.Id,
            Event = evt,
            CreatedAt = DateTime.UtcNow
        };

        var assignment1 = new StaffAssignment
        {
            Id = 101,
            StaffId = staff1.Id,
            Staff = staff1,
            ShiftId = shift.Id,
            Shift = shift,
            Status = AssignmentStatus.Assigned,
            AssignedAt = DateTime.UtcNow
        };

        var assignment2 = new StaffAssignment
        {
            Id = 102,
            StaffId = staff2.Id,
            Staff = staff2,
            ShiftId = shift.Id,
            Shift = shift,
            Status = AssignmentStatus.Assigned,
            AssignedAt = DateTime.UtcNow
        };

        shift.StaffAssignments = new List<StaffAssignment> { assignment1, assignment2 };

        return (shift, staff1, staff2);
    }

    [Fact]
    public async Task Handle_WhenShiftExistsWithAssignedStaff_SendsDeletionEmailToAllAssignedStaff()
    {
        // Arrange
        var (shift, staff1, staff2) = CreateShiftWithStaff();
        _mockRepository.Setup(r => r.GetShiftByIdAsync(shift.Id)).ReturnsAsync(shift);
        var command = new DeleteShiftCommand(shift.Id);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteShiftAsync(shift.Id), Times.Once);
        _mockEmailService.Verify(
            s => s.SendStaffAssignmentDeletionNotificationAsync(
                It.Is<Staff>(st => st.Id == staff1.Id),
                It.Is<Shift>(sh => sh.Id == shift.Id),
                It.Is<Event>(e => e.Id == shift.Event!.Id)),
            Times.Once);
        _mockEmailService.Verify(
            s => s.SendStaffAssignmentDeletionNotificationAsync(
                It.Is<Staff>(st => st.Id == staff2.Id),
                It.Is<Shift>(sh => sh.Id == shift.Id),
                It.Is<Event>(e => e.Id == shift.Event!.Id)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenShiftExistsWithoutStaff_DoesNotSendEmail()
    {
        // Arrange
        var evt = new Event
        {
            Id = 1,
            Name = "Festival Event",
            Location = "Main Square",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2)
        };
        var shift = new Shift
        {
            Id = 20,
            Name = "Night Shift",
            StartTime = evt.StartDate,
            EndTime = evt.StartDate.AddHours(8),
            EventId = evt.Id,
            Event = evt,
            StaffAssignments = new List<StaffAssignment>()
        };

        _mockRepository.Setup(r => r.GetShiftByIdAsync(shift.Id)).ReturnsAsync(shift);
        var command = new DeleteShiftCommand(shift.Id);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteShiftAsync(shift.Id), Times.Once);
        _mockEmailService.Verify(
            s => s.SendStaffAssignmentDeletionNotificationAsync(
                It.IsAny<Staff>(), It.IsAny<Shift>(), It.IsAny<Event>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenShiftDoesNotExist_DoesNotDeleteOrSendEmail()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetShiftByIdAsync(It.IsAny<int>())).ReturnsAsync((Shift?)null);
        var command = new DeleteShiftCommand(999);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.False(result);
        _mockRepository.Verify(r => r.DeleteShiftAsync(It.IsAny<int>()), Times.Never);
        _mockEmailService.Verify(
            s => s.SendStaffAssignmentDeletionNotificationAsync(
                It.IsAny<Staff>(), It.IsAny<Shift>(), It.IsAny<Event>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenEmailFails_ShiftDeletionStillSucceeds()
    {
        // Arrange
        var (shift, _, _) = CreateShiftWithStaff();
        _mockRepository.Setup(r => r.GetShiftByIdAsync(shift.Id)).ReturnsAsync(shift);
        _mockEmailService
            .Setup(s => s.SendStaffAssignmentDeletionNotificationAsync(
                It.IsAny<Staff>(), It.IsAny<Shift>(), It.IsAny<Event>()))
            .ThrowsAsync(new InvalidOperationException("SMTP connection error"));
        var command = new DeleteShiftCommand(shift.Id);

        // Act
        var result = await _handler.Handle(command);

        // Assert: Handler catches exception and shift deletion still succeeds
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteShiftAsync(shift.Id), Times.Once);
    }
}
