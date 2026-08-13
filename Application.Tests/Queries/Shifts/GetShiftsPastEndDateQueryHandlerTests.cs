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
    public async Task Handle_CallsRepositoryWithReferenceDate_AndReturnsRepositoryResult()
    {
        // Arrange
        var referenceDate = new DateTime(2026, 08, 13, 12, 0, 0, DateTimeKind.Utc);
        var shifts = new List<Shift>
        {
            new() { Id = 1, Name = "Past 1", EndTime = referenceDate.AddHours(-3) },
            new() { Id = 3, Name = "Past 2", EndTime = referenceDate.AddHours(-1) }
        };

        _mockShiftRepository
            .Setup(repository => repository.GetShiftsPastEndDateAsync(referenceDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shifts);

        // Act
        var result = await _handler.Handle(new GetShiftsPastEndDateQuery(referenceDate));

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal(3, result[1].Id);
        _mockShiftRepository.Verify(repository => repository.GetShiftsPastEndDateAsync(referenceDate, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNoPastShifts_ReturnsEmptyList()
    {
        // Arrange
        var referenceDate = new DateTime(2026, 08, 13, 12, 0, 0, DateTimeKind.Utc);
        _mockShiftRepository
            .Setup(repository => repository.GetShiftsPastEndDateAsync(referenceDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Shift>());

        // Act
        var result = await _handler.Handle(new GetShiftsPastEndDateQuery(referenceDate));

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WhenReferenceDateIsNull_UsesCurrentUtcDate()
    {
        // Arrange
        DateTime capturedReferenceDate = default;

        _mockShiftRepository
            .Setup(repository => repository.GetShiftsPastEndDateAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .Callback<DateTime, CancellationToken>((referenceDate, _) => capturedReferenceDate = referenceDate)
            .ReturnsAsync(new List<Shift>());

        var beforeCall = DateTime.UtcNow;

        // Act
        await _handler.Handle(new GetShiftsPastEndDateQuery(null));
        var afterCall = DateTime.UtcNow;

        // Assert
        Assert.InRange(capturedReferenceDate, beforeCall, afterCall);
    }
}
