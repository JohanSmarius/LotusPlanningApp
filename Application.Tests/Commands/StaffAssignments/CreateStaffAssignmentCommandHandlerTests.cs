using Application.Commands.StaffAssignments;
using Application.Common;
using Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Application.Tests.Commands.StaffAssignments;

public class CreateStaffAssignmentCommandHandlerTests
{
    private readonly Mock<IStaffAssignmentRepository> _mockAssignmentRepository;
    private readonly Mock<IShiftRepository> _mockShiftRepository;
    private readonly Mock<ILogger<CreateStaffAssignmentCommandHandler>> _mockLogger;
    private readonly CreateStaffAssignmentCommandHandler _handler;

    public CreateStaffAssignmentCommandHandlerTests()
    {
        _mockAssignmentRepository = new Mock<IStaffAssignmentRepository>();
        _mockShiftRepository = new Mock<IShiftRepository>();
        _mockLogger = new Mock<ILogger<CreateStaffAssignmentCommandHandler>>();
        _handler = new CreateStaffAssignmentCommandHandler(
            _mockAssignmentRepository.Object,
            _mockShiftRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShiftNotFound_ThrowsApplicationLayerException()
    {
        // Arrange
        var assignment = new StaffAssignment { ShiftId = 99, StaffId = 7, InvoiceEmail = "billing@example.com" };
        var command = new CreateStaffAssignmentCommand(assignment);

        _mockShiftRepository.Setup(r => r.GetShiftByIdAsync(99)).ReturnsAsync((Shift?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationLayerException>(() => _handler.Handle(command));
        _mockAssignmentRepository.Verify(r => r.CreateAssignmentAsync(It.IsAny<StaffAssignment>()), Times.Never);
    }

    [Fact]
    public async Task Handle_StaffUnavailable_ThrowsApplicationLayerException()
    {
        // Arrange
        var start = DateTime.UtcNow.AddHours(2);
        var end = start.AddHours(4);
        var shift = new Shift { Id = 5, EventId = 1, Name = "Night Shift", StartTime = start, EndTime = end, RequiredStaff = 2 };
        var assignment = new StaffAssignment { ShiftId = 5, StaffId = 12, InvoiceEmail = "billing@example.com" };
        var command = new CreateStaffAssignmentCommand(assignment);

        _mockShiftRepository.Setup(r => r.GetShiftByIdAsync(5)).ReturnsAsync(shift);
        _mockAssignmentRepository
            .Setup(r => r.IsStaffAvailableAsync(12, start, end, null))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationLayerException>(() => _handler.Handle(command));
        _mockAssignmentRepository.Verify(r => r.CreateAssignmentAsync(It.IsAny<StaffAssignment>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidAssignment_CreatesAssignmentWithInvoiceEmail()
    {
        // Arrange
        var start = DateTime.UtcNow.AddDays(1);
        var end = start.AddHours(3);
        var shift = new Shift { Id = 11, EventId = 2, Name = "Day Shift", StartTime = start, EndTime = end, RequiredStaff = 1 };
        var assignment = new StaffAssignment
        {
            ShiftId = 11,
            StaffId = 33,
            Role = StaffRole.Coordinator,
            InvoiceEmail = "finance@organization.org",
            Status = AssignmentStatus.Assigned
        };
        var command = new CreateStaffAssignmentCommand(assignment);

        _mockShiftRepository.Setup(r => r.GetShiftByIdAsync(11)).ReturnsAsync(shift);
        _mockAssignmentRepository
            .Setup(r => r.IsStaffAvailableAsync(33, start, end, null))
            .ReturnsAsync(true);
        _mockAssignmentRepository
            .Setup(r => r.CreateAssignmentAsync(It.IsAny<StaffAssignment>()))
            .ReturnsAsync((StaffAssignment created) =>
            {
                created.Id = 123;
                return created;
            });

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(123, result.Id);
        Assert.Equal("finance@organization.org", result.InvoiceEmail);

        _mockAssignmentRepository.Verify(r => r.CreateAssignmentAsync(
            It.Is<StaffAssignment>(a => a.InvoiceEmail == "finance@organization.org")), Times.Once);
    }
}
