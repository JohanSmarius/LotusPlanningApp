using Application.Queries.StaffAssignments;
using Entities;
using Moq;
using Xunit;

namespace Application.Tests.Queries.StaffAssignments;

public class GetAssignmentsByShiftIdQueryHandlerTests
{
    private readonly Mock<IStaffAssignmentRepository> _mockRepository;
    private readonly GetAssignmentsByShiftIdQueryHandler _handler;

    public GetAssignmentsByShiftIdQueryHandlerTests()
    {
        _mockRepository = new Mock<IStaffAssignmentRepository>();
        _handler = new GetAssignmentsByShiftIdQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public void Constructor_WithValidRepository_InitializesHandler()
    {
        // Arrange & Act
        var handler = new GetAssignmentsByShiftIdQueryHandler(_mockRepository.Object);

        // Assert
        Assert.NotNull(handler);
    }

    [Fact]
    public async Task Handle_WithValidShiftId_ReturnsAssignments()
    {
        // Arrange
        var shiftId = 1;
        var expectedAssignments = new List<StaffAssignment>
        {
            new StaffAssignment { Id = 1, ShiftId = shiftId, StaffId = 1 },
            new StaffAssignment { Id = 2, ShiftId = shiftId, StaffId = 2 }
        };
        var query = new GetAssignmentsByShiftIdQuery(shiftId);
        _mockRepository.Setup(r => r.GetAssignmentsByShiftIdAsync(shiftId))
            .ReturnsAsync(expectedAssignments);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(expectedAssignments, result);
        _mockRepository.Verify(r => r.GetAssignmentsByShiftIdAsync(shiftId), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNoAssignments_ReturnsEmptyList()
    {
        // Arrange
        var shiftId = 999;
        var emptyList = new List<StaffAssignment>();
        var query = new GetAssignmentsByShiftIdQuery(shiftId);
        _mockRepository.Setup(r => r.GetAssignmentsByShiftIdAsync(shiftId))
            .ReturnsAsync(emptyList);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockRepository.Verify(r => r.GetAssignmentsByShiftIdAsync(shiftId), Times.Once);
    }

    [Fact]
    public async Task Handle_WithCancellationToken_CallsRepositoryCorrectly()
    {
        // Arrange
        var shiftId = 5;
        var assignments = new List<StaffAssignment>
        {
            new StaffAssignment { Id = 10, ShiftId = shiftId, StaffId = 3 }
        };
        var query = new GetAssignmentsByShiftIdQuery(shiftId);
        var cancellationToken = new CancellationToken();
        _mockRepository.Setup(r => r.GetAssignmentsByShiftIdAsync(shiftId))
            .ReturnsAsync(assignments);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(assignments, result);
        _mockRepository.Verify(r => r.GetAssignmentsByShiftIdAsync(shiftId), Times.Once);
    }
}
