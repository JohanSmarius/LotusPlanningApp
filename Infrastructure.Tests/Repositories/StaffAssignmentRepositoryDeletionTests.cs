using Application;
using Entities;
using Infrastructure;
using LotusPlanningApp.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Infrastructure.Tests.Repositories;

/// <summary>
/// Tests that verify an email notification is sent when a staff assignment is deleted.
/// </summary>
public class StaffAssignmentRepositoryDeletionTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ApplicationDbContext _context;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<ILogger<StaffAssignmentRepository>> _mockLogger;
    private readonly StaffAssignmentRepository _repository;

    public StaffAssignmentRepositoryDeletionTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();

        _mockEmailService = new Mock<IEmailService>();
        _mockLogger = new Mock<ILogger<StaffAssignmentRepository>>();
        _repository = new StaffAssignmentRepository(_context, _mockEmailService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task DeleteAssignmentAsync_WhenAssignmentExists_SendsDeletionEmail()
    {
        // Arrange: seed the required entities.
        var staff = new Staff
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _context.Staff.Add(staff);

        var evt = new Event
        {
            Name = "Test Event",
            Location = "Venue A",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            CreatedAt = DateTime.UtcNow
        };
        _context.Events.Add(evt);
        await _context.SaveChangesAsync();

        var shift = new Shift
        {
            Name = "Morning Shift",
            StartTime = evt.StartDate,
            EndTime = evt.StartDate.AddHours(8),
            EventId = evt.Id,
            CreatedAt = DateTime.UtcNow
        };
        _context.Shifts.Add(shift);
        await _context.SaveChangesAsync();

        var assignment = new StaffAssignment
        {
            StaffId = staff.Id,
            ShiftId = shift.Id,
            Status = AssignmentStatus.Assigned,
            AssignedAt = DateTime.UtcNow
        };
        _context.StaffAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAssignmentAsync(assignment.Id);

        // Assert: assignment is removed from the database.
        var remaining = await _context.StaffAssignments.FindAsync(assignment.Id);
        Assert.Null(remaining);

        // Assert: deletion notification email was sent to the staff member.
        _mockEmailService.Verify(
            s => s.SendStaffAssignmentDeletionNotificationAsync(
                It.Is<Staff>(st => st.Id == staff.Id),
                It.Is<Shift>(sh => sh.Id == shift.Id),
                It.Is<Event>(e => e.Id == evt.Id)),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAssignmentAsync_WhenAssignmentDoesNotExist_DoesNotSendEmail()
    {
        // Act
        await _repository.DeleteAssignmentAsync(99999);

        // Assert: no email sent for non-existent assignment.
        _mockEmailService.Verify(
            s => s.SendStaffAssignmentDeletionNotificationAsync(
                It.IsAny<Staff>(), It.IsAny<Shift>(), It.IsAny<Event>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAssignmentAsync_WhenEmailFails_AssignmentIsStillDeleted()
    {
        // Arrange
        var staff = new Staff
        {
            FirstName = "John",
            LastName = "Smith",
            Email = "john.smith@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _context.Staff.Add(staff);

        var evt = new Event
        {
            Name = "Another Event",
            Location = "Venue B",
            StartDate = DateTime.UtcNow.AddDays(3),
            EndDate = DateTime.UtcNow.AddDays(4),
            CreatedAt = DateTime.UtcNow
        };
        _context.Events.Add(evt);
        await _context.SaveChangesAsync();

        var shift = new Shift
        {
            Name = "Evening Shift",
            StartTime = evt.StartDate,
            EndTime = evt.StartDate.AddHours(6),
            EventId = evt.Id,
            CreatedAt = DateTime.UtcNow
        };
        _context.Shifts.Add(shift);
        await _context.SaveChangesAsync();

        var assignment = new StaffAssignment
        {
            StaffId = staff.Id,
            ShiftId = shift.Id,
            Status = AssignmentStatus.Assigned,
            AssignedAt = DateTime.UtcNow
        };
        _context.StaffAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        _mockEmailService
            .Setup(s => s.SendStaffAssignmentDeletionNotificationAsync(
                It.IsAny<Staff>(), It.IsAny<Shift>(), It.IsAny<Event>()))
            .ThrowsAsync(new InvalidOperationException("SMTP error"));

        // Act: should not throw even when email fails.
        await _repository.DeleteAssignmentAsync(assignment.Id);

        // Assert: assignment is still removed.
        var remaining = await _context.StaffAssignments.FindAsync(assignment.Id);
        Assert.Null(remaining);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
