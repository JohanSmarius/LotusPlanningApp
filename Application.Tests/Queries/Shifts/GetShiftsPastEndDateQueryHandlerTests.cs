using Application;
using Application.Queries.Shifts;
using Entities;
using Moq;
using Xunit;

namespace Application.Tests.Queries.Shifts;

public class GetShiftsPastEndDateQueryHandlerTests
{
    private readonly Mock<IShiftRepository> _mockShiftRepository;
    private readonly GetShiftsPastEndDateQueryHandler _handler;

    public GetShiftsPastEndDateQueryHandlerTests()
    {
        _mockShiftRepository = new Mock<IShiftRepository>();
        _handler = new GetShiftsPastEndDateQueryHandler(_mockShiftRepository.Object);
    }

    [Fact]
    public async Task Handle_ReturnsOnlyShiftsPastReferenceDate_OrderedByLatestEndDateFirst()
    {
        // Arrange
        var referenceDate = new DateTime(2026, 08, 13, 12, 0, 0, DateTimeKind.Utc);
        var shifts = new List<Shift>
        {
            new() { Id = 1, Name = "Past 1", EndTime = referenceDate.AddHours(-3) },
            new() { Id = 2, Name = "Future", EndTime = referenceDate.AddHours(1) },
            new() { Id = 3, Name = "Past 2", EndTime = referenceDate.AddHours(-1) }
        };

        _mockShiftRepository
            .Setup(repository => repository.GetAllShiftsAsync())
            .ReturnsAsync(shifts);

        // Act
        var result = await _handler.Handle(new GetShiftsPastEndDateQuery(referenceDate));

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(3, result[0].Id);
        Assert.Equal(1, result[1].Id);
    }

    [Fact]
    public async Task Handle_WithNoPastShifts_ReturnsEmptyList()
    {
        // Arrange
        var referenceDate = new DateTime(2026, 08, 13, 12, 0, 0, DateTimeKind.Utc);
        var shifts = new List<Shift>
        {
            new() { Id = 1, Name = "Future 1", EndTime = referenceDate.AddHours(1) },
            new() { Id = 2, Name = "Future 2", EndTime = referenceDate.AddHours(2) }
        };

        _mockShiftRepository
            .Setup(repository => repository.GetAllShiftsAsync())
            .ReturnsAsync(shifts);

        // Act
        var result = await _handler.Handle(new GetShiftsPastEndDateQuery(referenceDate));

        // Assert
        Assert.Empty(result);
    }
}
