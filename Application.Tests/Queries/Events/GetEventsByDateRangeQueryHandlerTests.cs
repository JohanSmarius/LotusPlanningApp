using Application.Queries.Events;
using Entities;
using Moq;
using Xunit;

namespace Application.Tests.Queries.Events;

public class GetEventsByDateRangeQueryHandlerTests
{
    private readonly Mock<IEventRepository> _mockRepository;
    private readonly GetEventsByDateRangeQueryHandler _handler;

    public GetEventsByDateRangeQueryHandlerTests()
    {
        _mockRepository = new Mock<IEventRepository>();
        _handler = new GetEventsByDateRangeQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public void Constructor_WithValidRepository_InitializesHandler()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();

        // Act
        var handler = new GetEventsByDateRangeQueryHandler(mockRepository.Object);

        // Assert
        Assert.NotNull(handler);
    }

    [Fact]
    public async Task Handle_WithValidDateRange_ReturnsEvents()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 1, 31);
        var expectedEvents = new List<Event>
        {
            new Event { Id = 1, Name = "Event 1", StartDate = new DateTime(2024, 1, 10), EndDate = new DateTime(2024, 1, 10), Location = "Location 1" },
            new Event { Id = 2, Name = "Event 2", StartDate = new DateTime(2024, 1, 20), EndDate = new DateTime(2024, 1, 20), Location = "Location 2" }
        };
        var query = new GetEventsByDateRangeQuery(startDate, endDate);
        _mockRepository.Setup(r => r.GetEventsByDateRangeAsync(startDate, endDate))
            .ReturnsAsync(expectedEvents);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(expectedEvents, result);
        _mockRepository.Verify(r => r.GetEventsByDateRangeAsync(startDate, endDate), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNoEventsInRange_ReturnsEmptyList()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 1, 31);
        var emptyList = new List<Event>();
        var query = new GetEventsByDateRangeQuery(startDate, endDate);
        _mockRepository.Setup(r => r.GetEventsByDateRangeAsync(startDate, endDate))
            .ReturnsAsync(emptyList);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockRepository.Verify(r => r.GetEventsByDateRangeAsync(startDate, endDate), Times.Once);
    }

    [Fact]
    public async Task Handle_WithSameDateRange_ReturnsEvents()
    {
        // Arrange
        var sameDate = new DateTime(2024, 1, 15);
        var expectedEvents = new List<Event>
        {
            new Event { Id = 1, Name = "Event 1", StartDate = sameDate, EndDate = sameDate, Location = "Location 1" }
        };
        var query = new GetEventsByDateRangeQuery(sameDate, sameDate);
        _mockRepository.Setup(r => r.GetEventsByDateRangeAsync(sameDate, sameDate))
            .ReturnsAsync(expectedEvents);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(expectedEvents, result);
        _mockRepository.Verify(r => r.GetEventsByDateRangeAsync(sameDate, sameDate), Times.Once);
    }

    [Fact]
    public async Task Handle_WithCancellationToken_PassesToRepository()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 1, 31);
        var expectedEvents = new List<Event>
        {
            new Event { Id = 1, Name = "Event 1", StartDate = new DateTime(2024, 1, 10), EndDate = new DateTime(2024, 1, 10), Location = "Location 1" }
        };
        var query = new GetEventsByDateRangeQuery(startDate, endDate);
        var cancellationToken = new CancellationToken();
        _mockRepository.Setup(r => r.GetEventsByDateRangeAsync(startDate, endDate))
            .ReturnsAsync(expectedEvents);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        _mockRepository.Verify(r => r.GetEventsByDateRangeAsync(startDate, endDate), Times.Once);
    }

    [Fact]
    public async Task Handle_WithMultipleEvents_ReturnsAllEvents()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 12, 31);
        var expectedEvents = new List<Event>
        {
            new Event { Id = 1, Name = "Event 1", StartDate = new DateTime(2024, 1, 10), EndDate = new DateTime(2024, 1, 10), Location = "Location 1" },
            new Event { Id = 2, Name = "Event 2", StartDate = new DateTime(2024, 3, 15), EndDate = new DateTime(2024, 3, 15), Location = "Location 2" },
            new Event { Id = 3, Name = "Event 3", StartDate = new DateTime(2024, 6, 20), EndDate = new DateTime(2024, 6, 20), Location = "Location 3" },
            new Event { Id = 4, Name = "Event 4", StartDate = new DateTime(2024, 12, 25), EndDate = new DateTime(2024, 12, 25), Location = "Location 4" }
        };
        var query = new GetEventsByDateRangeQuery(startDate, endDate);
        _mockRepository.Setup(r => r.GetEventsByDateRangeAsync(startDate, endDate))
            .ReturnsAsync(expectedEvents);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Count);
        Assert.Equal(expectedEvents, result);
        _mockRepository.Verify(r => r.GetEventsByDateRangeAsync(startDate, endDate), Times.Once);
    }
}
