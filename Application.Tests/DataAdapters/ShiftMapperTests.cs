using Application.DataAdapters;
using Entities;
using Xunit;

namespace Application.Tests.DataAdapters;

public class ShiftMapperTests
{
    [Fact]
    public void ToDTO_ValidShift_MapsAllProperties()
    {
        // Arrange
        var createdAt = DateTime.UtcNow.AddDays(-1);
        var updatedAt = DateTime.UtcNow;
        var startTime = DateTime.UtcNow.AddHours(1);
        var endTime = DateTime.UtcNow.AddHours(5);

        var entityShift = new Shift
        {
            Id = 1,
            EventId = 10,
            Name = "Morning Shift",
            StartTime = startTime,
            EndTime = endTime,
            RequiredStaff = 5,
            Description = "Test description",
            Status = ShiftStatus.Open,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        // Act
        var result = entityShift.ToDTO();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(10, result.EventId);
        Assert.Equal("Morning Shift", result.Name);
        Assert.Equal(startTime, result.StartTime);
        Assert.Equal(endTime, result.EndTime);
        Assert.Equal(5, result.RequiredStaff);
        Assert.Equal("Test description", result.Description);
        Assert.Equal(ShiftStatusDTO.Open, result.Status);
        Assert.Equal(createdAt, result.CreatedAt);
        Assert.Equal(updatedAt, result.UpdatedAt);
    }

    [Fact]
    public void ToDTO_ShiftWithNullDescription_MapsCorrectly()
    {
        // Arrange
        var entityShift = new Shift
        {
            Id = 2,
            EventId = 20,
            Name = "Evening Shift",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(4),
            RequiredStaff = 3,
            Description = null,
            Status = ShiftStatus.Full,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        // Act
        var result = entityShift.ToDTO();

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Description);
        Assert.Null(result.UpdatedAt);
        Assert.Equal(ShiftStatusDTO.Full, result.Status);
    }

    [Theory]
    [InlineData(ShiftStatus.Open, ShiftStatusDTO.Open)]
    [InlineData(ShiftStatus.Full, ShiftStatusDTO.Full)]
    [InlineData(ShiftStatus.InProgress, ShiftStatusDTO.InProgress)]
    [InlineData(ShiftStatus.Completed, ShiftStatusDTO.Completed)]
    [InlineData(ShiftStatus.Cancelled, ShiftStatusDTO.Cancelled)]
    public void ToDTO_AllShiftStatuses_MapCorrectly(ShiftStatus entityStatus, ShiftStatusDTO expectedDtoStatus)
    {
        // Arrange
        var entityShift = new Shift
        {
            Id = 3,
            EventId = 30,
            Name = "Test Shift",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(2),
            Status = entityStatus,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = entityShift.ToDTO();

        // Assert
        Assert.Equal(expectedDtoStatus, result.Status);
    }

    [Fact]
    public void ToEntity_ValidShiftDTO_MapsAllProperties()
    {
        // Arrange
        var createdAt = DateTime.UtcNow.AddDays(-2);
        var updatedAt = DateTime.UtcNow.AddDays(-1);
        var startTime = DateTime.UtcNow.AddHours(2);
        var endTime = DateTime.UtcNow.AddHours(6);

        var dtoShift = new ShiftDTO
        {
            Id = 5,
            EventId = 50,
            Name = "Night Shift",
            StartTime = startTime,
            EndTime = endTime,
            RequiredStaff = 7,
            Description = "Night shift description",
            Status = ShiftStatusDTO.InProgress,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        // Act
        var result = dtoShift.ToEntity();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Id);
        Assert.Equal(50, result.EventId);
        Assert.Equal("Night Shift", result.Name);
        Assert.Equal(startTime, result.StartTime);
        Assert.Equal(endTime, result.EndTime);
        Assert.Equal(7, result.RequiredStaff);
        Assert.Equal("Night shift description", result.Description);
        Assert.Equal(ShiftStatus.InProgress, result.Status);
        Assert.Equal(createdAt, result.CreatedAt);
        Assert.Equal(updatedAt, result.UpdatedAt);
    }

    [Fact]
    public void ToEntity_ShiftDTOWithNullDescription_MapsCorrectly()
    {
        // Arrange
        var dtoShift = new ShiftDTO
        {
            Id = 6,
            EventId = 60,
            Name = "Afternoon Shift",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(3),
            RequiredStaff = 2,
            Description = null,
            Status = ShiftStatusDTO.Completed,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        // Act
        var result = dtoShift.ToEntity();

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Description);
        Assert.Null(result.UpdatedAt);
        Assert.Equal(ShiftStatus.Completed, result.Status);
    }

    [Theory]
    [InlineData(ShiftStatusDTO.Open, ShiftStatus.Open)]
    [InlineData(ShiftStatusDTO.Full, ShiftStatus.Full)]
    [InlineData(ShiftStatusDTO.InProgress, ShiftStatus.InProgress)]
    [InlineData(ShiftStatusDTO.Completed, ShiftStatus.Completed)]
    [InlineData(ShiftStatusDTO.Cancelled, ShiftStatus.Cancelled)]
    public void ToEntity_AllShiftStatusDTOs_MapCorrectly(ShiftStatusDTO dtoStatus, ShiftStatus expectedEntityStatus)
    {
        // Arrange
        var dtoShift = new ShiftDTO
        {
            Id = 7,
            EventId = 70,
            Name = "Test Shift",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(2),
            Status = dtoStatus,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = dtoShift.ToEntity();

        // Assert
        Assert.Equal(expectedEntityStatus, result.Status);
    }

    [Fact]
    public void ToDTOList_EmptyList_ReturnsEmptyList()
    {
        // Arrange
        var entityShifts = new List<Shift>();

        // Act
        var result = entityShifts.ToDTOList();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ToDTOList_MultipleShifts_MapsAllShifts()
    {
        // Arrange
        var entityShifts = new List<Shift>
        {
            new Shift
            {
                Id = 1,
                EventId = 10,
                Name = "Shift 1",
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(2),
                RequiredStaff = 3,
                Status = ShiftStatus.Open,
                CreatedAt = DateTime.UtcNow
            },
            new Shift
            {
                Id = 2,
                EventId = 20,
                Name = "Shift 2",
                StartTime = DateTime.UtcNow.AddHours(3),
                EndTime = DateTime.UtcNow.AddHours(5),
                RequiredStaff = 4,
                Status = ShiftStatus.Full,
                CreatedAt = DateTime.UtcNow
            },
            new Shift
            {
                Id = 3,
                EventId = 30,
                Name = "Shift 3",
                StartTime = DateTime.UtcNow.AddHours(6),
                EndTime = DateTime.UtcNow.AddHours(8),
                RequiredStaff = 2,
                Status = ShiftStatus.Completed,
                CreatedAt = DateTime.UtcNow
            }
        };

        // Act
        var result = entityShifts.ToDTOList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("Shift 1", result[0].Name);
        Assert.Equal(ShiftStatusDTO.Open, result[0].Status);
        Assert.Equal(2, result[1].Id);
        Assert.Equal("Shift 2", result[1].Name);
        Assert.Equal(ShiftStatusDTO.Full, result[1].Status);
        Assert.Equal(3, result[2].Id);
        Assert.Equal("Shift 3", result[2].Name);
        Assert.Equal(ShiftStatusDTO.Completed, result[2].Status);
    }

    [Fact]
    public void ToDTOList_SingleShift_ReturnsSingleItemList()
    {
        // Arrange
        var entityShifts = new List<Shift>
        {
            new Shift
            {
                Id = 100,
                EventId = 200,
                Name = "Solo Shift",
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(4),
                RequiredStaff = 1,
                Description = "Single shift test",
                Status = ShiftStatus.InProgress,
                CreatedAt = DateTime.UtcNow
            }
        };

        // Act
        var result = entityShifts.ToDTOList();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(100, result[0].Id);
        Assert.Equal("Solo Shift", result[0].Name);
        Assert.Equal("Single shift test", result[0].Description);
    }

    [Fact]
    public void ToEntityList_EmptyList_ReturnsEmptyList()
    {
        // Arrange
        var dtoShifts = new List<ShiftDTO>();

        // Act
        var result = dtoShifts.ToEntityList();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ToEntityList_MultipleShiftDTOs_MapsAllShifts()
    {
        // Arrange
        var dtoShifts = new List<ShiftDTO>
        {
            new ShiftDTO
            {
                Id = 11,
                EventId = 110,
                Name = "DTO Shift 1",
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(2),
                RequiredStaff = 5,
                Status = ShiftStatusDTO.Open,
                CreatedAt = DateTime.UtcNow
            },
            new ShiftDTO
            {
                Id = 12,
                EventId = 120,
                Name = "DTO Shift 2",
                StartTime = DateTime.UtcNow.AddHours(3),
                EndTime = DateTime.UtcNow.AddHours(6),
                RequiredStaff = 6,
                Status = ShiftStatusDTO.Cancelled,
                CreatedAt = DateTime.UtcNow
            },
            new ShiftDTO
            {
                Id = 13,
                EventId = 130,
                Name = "DTO Shift 3",
                StartTime = DateTime.UtcNow.AddHours(7),
                EndTime = DateTime.UtcNow.AddHours(9),
                RequiredStaff = 3,
                Status = ShiftStatusDTO.InProgress,
                CreatedAt = DateTime.UtcNow
            }
        };

        // Act
        var result = dtoShifts.ToEntityList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal(11, result[0].Id);
        Assert.Equal("DTO Shift 1", result[0].Name);
        Assert.Equal(ShiftStatus.Open, result[0].Status);
        Assert.Equal(12, result[1].Id);
        Assert.Equal("DTO Shift 2", result[1].Name);
        Assert.Equal(ShiftStatus.Cancelled, result[1].Status);
        Assert.Equal(13, result[2].Id);
        Assert.Equal("DTO Shift 3", result[2].Name);
        Assert.Equal(ShiftStatus.InProgress, result[2].Status);
    }

    [Fact]
    public void ToEntityList_SingleShiftDTO_ReturnsSingleItemList()
    {
        // Arrange
        var dtoShifts = new List<ShiftDTO>
        {
            new ShiftDTO
            {
                Id = 150,
                EventId = 250,
                Name = "Single DTO Shift",
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(5),
                RequiredStaff = 8,
                Description = "Single DTO test",
                Status = ShiftStatusDTO.Full,
                CreatedAt = DateTime.UtcNow
            }
        };

        // Act
        var result = dtoShifts.ToEntityList();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(150, result[0].Id);
        Assert.Equal("Single DTO Shift", result[0].Name);
        Assert.Equal("Single DTO test", result[0].Description);
        Assert.Equal(ShiftStatus.Full, result[0].Status);
    }
}
