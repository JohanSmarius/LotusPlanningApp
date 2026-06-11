using Application.DataAdapters;
using Entities;
using Xunit;

namespace Application.Tests.DataAdapters;

public class EventMapperTests
{
    [Fact]
    public void ToDTO_ValidEventWithAllProperties_MapsAllProperties()
    {
        // Arrange
        var entityEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = new DateTime(2024, 1, 1, 10, 0, 0),
            EndDate = new DateTime(2024, 1, 1, 18, 0, 0),
            Location = "Test Location",
            Description = "Test Description",
            Status = EventStatus.Confirmed,
            ContactPerson = "John Doe",
            ContactPhone = "123-456-7890",
            ContactEmail = "john@example.com",
            NotificationSent = true,
            RequiredStaffCount = 5,
            CustomerId = 10,
            CancellationRequested = true,
            Shifts = new List<Shift>
            {
                new Shift
                {
                    Id = 1,
                    EventId = 1,
                    Name = "Morning Shift",
                    StartTime = new DateTime(2024, 1, 1, 10, 0, 0),
                    EndTime = new DateTime(2024, 1, 1, 14, 0, 0),
                    RequiredStaff = 2,
                    Description = "Morning",
                    Status = ShiftStatus.Open
                }
            }
        };

        // Act
        var result = entityEvent.ToDTO();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Event", result.Name);
        Assert.Equal(new DateTime(2024, 1, 1, 10, 0, 0), result.StartDate);
        Assert.Equal(new DateTime(2024, 1, 1, 18, 0, 0), result.EndDate);
        Assert.Equal("Test Location", result.Location);
        Assert.Equal("Test Description", result.Description);
        Assert.Equal(EventStatusDTO.Confirmed, result.Status);
        Assert.Equal("John Doe", result.ContactPerson);
        Assert.Equal("123-456-7890", result.ContactPhone);
        Assert.Equal("john@example.com", result.ContactEmail);
        Assert.True(result.NotificationSent);
        Assert.Equal(5, result.RequiredStaffCount);
        Assert.Equal(10, result.CustomerId);
        Assert.True(result.CancellationRequested);
        Assert.NotNull(result.Shifts);
        Assert.Single(result.Shifts);
        Assert.Equal(1, result.Shifts[0].Id);
        Assert.Equal("Morning Shift", result.Shifts[0].Name);
    }

    [Fact]
    public void ToDTO_EventWithNullShifts_ReturnsEmptyShiftsList()
    {
        // Arrange
        var entityEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = new DateTime(2024, 1, 1, 10, 0, 0),
            EndDate = new DateTime(2024, 1, 1, 18, 0, 0),
            Location = "Test Location",
            Status = EventStatus.Requested,
            RequiredStaffCount = 1,
            Shifts = null!
        };

        // Act
        var result = entityEvent.ToDTO();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Shifts);
        Assert.Empty(result.Shifts);
    }

    [Fact]
    public void ToDTO_EventWithNullablePropertiesNull_MapsNullValues()
    {
        // Arrange
        var entityEvent = new Event
        {
            Id = 2,
            Name = "Minimal Event",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 1, 2),
            Location = "Location",
            Description = null,
            Status = EventStatus.Planned,
            ContactPerson = null,
            ContactPhone = null,
            ContactEmail = null,
            NotificationSent = false,
            RequiredStaffCount = 1,
            CustomerId = null,
            CancellationRequested = false,
            Shifts = new List<Shift>()
        };

        // Act
        var result = entityEvent.ToDTO();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Id);
        Assert.Null(result.Description);
        Assert.Null(result.ContactPerson);
        Assert.Null(result.ContactPhone);
        Assert.Null(result.ContactEmail);
        Assert.Null(result.CustomerId);
        Assert.False(result.NotificationSent);
        Assert.False(result.CancellationRequested);
    }

    [Fact]
    public void ToDTO_EventWithMultipleShifts_MapsAllShifts()
    {
        // Arrange
        var entityEvent = new Event
        {
            Id = 3,
            Name = "Multi Shift Event",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 1, 2),
            Location = "Location",
            Status = EventStatus.Active,
            RequiredStaffCount = 10,
            Shifts = new List<Shift>
            {
                new Shift { Id = 1, Name = "Shift 1", StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(4), RequiredStaff = 3, EventId = 3, Status = ShiftStatus.Open },
                new Shift { Id = 2, Name = "Shift 2", StartTime = DateTime.Now.AddHours(4), EndTime = DateTime.Now.AddHours(8), RequiredStaff = 3, EventId = 3, Status = ShiftStatus.Full },
                new Shift { Id = 3, Name = "Shift 3", StartTime = DateTime.Now.AddHours(8), EndTime = DateTime.Now.AddHours(12), RequiredStaff = 4, EventId = 3, Status = ShiftStatus.InProgress }
            }
        };

        // Act
        var result = entityEvent.ToDTO();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Shifts);
        Assert.Equal(3, result.Shifts.Count);
        Assert.Equal("Shift 1", result.Shifts[0].Name);
        Assert.Equal("Shift 2", result.Shifts[1].Name);
        Assert.Equal("Shift 3", result.Shifts[2].Name);
    }

    [Fact]
    public void ToEntity_ValidDTOWithAllProperties_MapsAllProperties()
    {
        // Arrange
        var dtoEvent = new EventDTO
        {
            Id = 1,
            Name = "Test Event",
            StartDate = new DateTime(2024, 1, 1, 10, 0, 0),
            EndDate = new DateTime(2024, 1, 1, 18, 0, 0),
            Location = "Test Location",
            Description = "Test Description",
            Status = EventStatusDTO.Confirmed,
            ContactPerson = "John Doe",
            ContactPhone = "123-456-7890",
            ContactEmail = "john@example.com",
            NotificationSent = true,
            RequiredStaffCount = 5,
            CustomerId = 10,
            CancellationRequested = true,
            Shifts = new List<ShiftDTO>
            {
                new ShiftDTO
                {
                    Id = 1,
                    EventId = 1,
                    Name = "Morning Shift",
                    StartTime = new DateTime(2024, 1, 1, 10, 0, 0),
                    EndTime = new DateTime(2024, 1, 1, 14, 0, 0),
                    RequiredStaff = 2,
                    Description = "Morning",
                    Status = ShiftStatusDTO.Open
                }
            }
        };

        // Act
        var result = dtoEvent.ToEntity();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Event", result.Name);
        Assert.Equal(new DateTime(2024, 1, 1, 10, 0, 0), result.StartDate);
        Assert.Equal(new DateTime(2024, 1, 1, 18, 0, 0), result.EndDate);
        Assert.Equal("Test Location", result.Location);
        Assert.Equal("Test Description", result.Description);
        Assert.Equal(EventStatus.Confirmed, result.Status);
        Assert.Equal("John Doe", result.ContactPerson);
        Assert.Equal("123-456-7890", result.ContactPhone);
        Assert.Equal("john@example.com", result.ContactEmail);
        Assert.True(result.NotificationSent);
        Assert.Equal(5, result.RequiredStaffCount);
        Assert.Equal(10, result.CustomerId);
        Assert.True(result.CancellationRequested);
        Assert.NotNull(result.Shifts);
        Assert.Single(result.Shifts);
        Assert.Equal(1, result.Shifts[0].Id);
        Assert.Equal("Morning Shift", result.Shifts[0].Name);
    }

    [Fact]
    public void ToEntity_DTOWithNullShifts_ReturnsEmptyShiftsList()
    {
        // Arrange
        var dtoEvent = new EventDTO
        {
            Id = 1,
            Name = "Test Event",
            StartDate = new DateTime(2024, 1, 1, 10, 0, 0),
            EndDate = new DateTime(2024, 1, 1, 18, 0, 0),
            Location = "Test Location",
            Status = EventStatusDTO.Requested,
            RequiredStaffCount = 1,
            Shifts = null!
        };

        // Act
        var result = dtoEvent.ToEntity();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Shifts);
        Assert.Empty(result.Shifts);
    }

    [Fact]
    public void ToEntity_DTOWithNullablePropertiesNull_MapsNullValues()
    {
        // Arrange
        var dtoEvent = new EventDTO
        {
            Id = 2,
            Name = "Minimal Event",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 1, 2),
            Location = "Location",
            Description = null,
            Status = EventStatusDTO.Planned,
            ContactPerson = null,
            ContactPhone = null,
            ContactEmail = null,
            NotificationSent = false,
            RequiredStaffCount = 1,
            CustomerId = null,
            CancellationRequested = false,
            Shifts = new List<ShiftDTO>()
        };

        // Act
        var result = dtoEvent.ToEntity();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Id);
        Assert.Null(result.Description);
        Assert.Null(result.ContactPerson);
        Assert.Null(result.ContactPhone);
        Assert.Null(result.ContactEmail);
        Assert.Null(result.CustomerId);
        Assert.False(result.NotificationSent);
        Assert.False(result.CancellationRequested);
    }

    [Fact]
    public void ToEntity_DTOWithMultipleShifts_MapsAllShifts()
    {
        // Arrange
        var dtoEvent = new EventDTO
        {
            Id = 3,
            Name = "Multi Shift Event",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 1, 2),
            Location = "Location",
            Status = EventStatusDTO.Active,
            RequiredStaffCount = 10,
            Shifts = new List<ShiftDTO>
            {
                new ShiftDTO { Id = 1, Name = "Shift 1", StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(4), RequiredStaff = 3, EventId = 3, Status = ShiftStatusDTO.Open },
                new ShiftDTO { Id = 2, Name = "Shift 2", StartTime = DateTime.Now.AddHours(4), EndTime = DateTime.Now.AddHours(8), RequiredStaff = 3, EventId = 3, Status = ShiftStatusDTO.Full },
                new ShiftDTO { Id = 3, Name = "Shift 3", StartTime = DateTime.Now.AddHours(8), EndTime = DateTime.Now.AddHours(12), RequiredStaff = 4, EventId = 3, Status = ShiftStatusDTO.InProgress }
            }
        };

        // Act
        var result = dtoEvent.ToEntity();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Shifts);
        Assert.Equal(3, result.Shifts.Count);
        Assert.Equal("Shift 1", result.Shifts[0].Name);
        Assert.Equal("Shift 2", result.Shifts[1].Name);
        Assert.Equal("Shift 3", result.Shifts[2].Name);
    }

    [Fact]
    public void ToDTO_AllEventStatuses_MapsCorrectly()
    {
        // Arrange & Act & Assert
        var requestedEvent = new Event { Id = 1, Name = "E1", StartDate = DateTime.Now, EndDate = DateTime.Now, Location = "L1", Status = EventStatus.Requested, RequiredStaffCount = 1 };
        Assert.Equal(EventStatusDTO.Requested, requestedEvent.ToDTO().Status);

        var plannedEvent = new Event { Id = 2, Name = "E2", StartDate = DateTime.Now, EndDate = DateTime.Now, Location = "L2", Status = EventStatus.Planned, RequiredStaffCount = 1 };
        Assert.Equal(EventStatusDTO.Planned, plannedEvent.ToDTO().Status);

        var confirmedEvent = new Event { Id = 3, Name = "E3", StartDate = DateTime.Now, EndDate = DateTime.Now, Location = "L3", Status = EventStatus.Confirmed, RequiredStaffCount = 1 };
        Assert.Equal(EventStatusDTO.Confirmed, confirmedEvent.ToDTO().Status);

        var activeEvent = new Event { Id = 4, Name = "E4", StartDate = DateTime.Now, EndDate = DateTime.Now, Location = "L4", Status = EventStatus.Active, RequiredStaffCount = 1 };
        Assert.Equal(EventStatusDTO.Active, activeEvent.ToDTO().Status);
    }

    [Fact]
    public void ToEntity_AllEventStatuses_MapsCorrectly()
    {
        // Arrange & Act & Assert
        var requestedDTO = new EventDTO { Id = 1, Name = "E1", StartDate = DateTime.Now, EndDate = DateTime.Now, Location = "L1", Status = EventStatusDTO.Requested, RequiredStaffCount = 1 };
        Assert.Equal(EventStatus.Requested, requestedDTO.ToEntity().Status);

        var plannedDTO = new EventDTO { Id = 2, Name = "E2", StartDate = DateTime.Now, EndDate = DateTime.Now, Location = "L2", Status = EventStatusDTO.Planned, RequiredStaffCount = 1 };
        Assert.Equal(EventStatus.Planned, plannedDTO.ToEntity().Status);

        var confirmedDTO = new EventDTO { Id = 3, Name = "E3", StartDate = DateTime.Now, EndDate = DateTime.Now, Location = "L3", Status = EventStatusDTO.Confirmed, RequiredStaffCount = 1 };
        Assert.Equal(EventStatus.Confirmed, confirmedDTO.ToEntity().Status);

        var activeDTO = new EventDTO { Id = 4, Name = "E4", StartDate = DateTime.Now, EndDate = DateTime.Now, Location = "L4", Status = EventStatusDTO.Active, RequiredStaffCount = 1 };
        Assert.Equal(EventStatus.Active, activeDTO.ToEntity().Status);
    }
}
