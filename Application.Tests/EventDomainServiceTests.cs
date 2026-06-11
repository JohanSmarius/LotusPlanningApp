using Application;
using Entities;
using Xunit;

namespace Application.Tests;

public class EventDomainServiceTests
{
    private readonly EventDomainService _service;

    public EventDomainServiceTests()
    {
        _service = new EventDomainService();
    }

    [Fact]
    public void ApplyChanges_MismatchedIds_ThrowsInvalidOperationException()
    {
        // Arrange
        var existing = new Event { Id = 1 };
        var updated = new Event { Id = 2 };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => _service.ApplyChanges(existing, updated));
        Assert.Equal("Mismatched Event ids.", exception.Message);
    }

    [Fact]
    public void ApplyChanges_CopiesAllMutableFields_UpdatesExistingEvent()
    {
        // Arrange
        var existing = new Event
        {
            Id = 1,
            Name = "Old Name",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 1, 2),
            Location = "Old Location",
            Description = "Old Description",
            Status = EventStatus.Requested,
            ContactPerson = "Old Person",
            ContactPhone = "123456",
            ContactEmail = "old@example.com",
            NotificationSent = false
        };

        var updated = new Event
        {
            Id = 1,
            Name = "New Name",
            StartDate = new DateTime(2024, 2, 1),
            EndDate = new DateTime(2024, 2, 2),
            Location = "New Location",
            Description = "New Description",
            Status = EventStatus.Confirmed,
            ContactPerson = "New Person",
            ContactPhone = "789012",
            ContactEmail = "new@example.com"
        };

        // Act
        _service.ApplyChanges(existing, updated);

        // Assert
        Assert.Equal("New Name", existing.Name);
        Assert.Equal(new DateTime(2024, 2, 1), existing.StartDate);
        Assert.Equal(new DateTime(2024, 2, 2), existing.EndDate);
        Assert.Equal("New Location", existing.Location);
        Assert.Equal("New Description", existing.Description);
        Assert.Equal(EventStatus.Confirmed, existing.Status);
        Assert.Equal("New Person", existing.ContactPerson);
        Assert.Equal("789012", existing.ContactPhone);
        Assert.Equal("new@example.com", existing.ContactEmail);
        Assert.NotNull(existing.UpdatedAt);
    }

    [Fact]
    public void ApplyChanges_TransitionToPlanned_WithContactEmailAndNoNotificationSent_ReturnsSendPlannedTrue()
    {
        // Arrange
        var existing = new Event
        {
            Id = 1,
            Status = EventStatus.Requested,
            NotificationSent = false,
            ContactEmail = "contact@example.com"
        };

        var updated = new Event
        {
            Id = 1,
            Status = EventStatus.Planned,
            ContactEmail = "contact@example.com"
        };

        // Act
        var decision = _service.ApplyChanges(existing, updated);

        // Assert
        Assert.True(decision.ShouldSendPlannedNotification);
        Assert.True(decision.PromoteToConfirmedAfterPlanned);
        Assert.False(decision.ShouldSendInvoiceNotification);
    }

    [Fact]
    public void ApplyChanges_TransitionToPlanned_WithoutContactEmail_ReturnsSendPlannedFalse()
    {
        // Arrange
        var existing = new Event
        {
            Id = 1,
            Status = EventStatus.Requested,
            NotificationSent = false,
            ContactEmail = null
        };

        var updated = new Event
        {
            Id = 1,
            Status = EventStatus.Planned,
            ContactEmail = null
        };

        // Act
        var decision = _service.ApplyChanges(existing, updated);

        // Assert
        Assert.False(decision.ShouldSendPlannedNotification);
        Assert.False(decision.PromoteToConfirmedAfterPlanned);
    }

    [Fact]
    public void ApplyChanges_TransitionToPlanned_WithEmptyContactEmail_ReturnsSendPlannedFalse()
    {
        // Arrange
        var existing = new Event
        {
            Id = 1,
            Status = EventStatus.Requested,
            NotificationSent = false,
            ContactEmail = ""
        };

        var updated = new Event
        {
            Id = 1,
            Status = EventStatus.Planned,
            ContactEmail = ""
        };

        // Act
        var decision = _service.ApplyChanges(existing, updated);

        // Assert
        Assert.False(decision.ShouldSendPlannedNotification);
        Assert.False(decision.PromoteToConfirmedAfterPlanned);
    }

    [Fact]
    public void ApplyChanges_TransitionToPlanned_WithWhitespaceContactEmail_ReturnsSendPlannedFalse()
    {
        // Arrange
        var existing = new Event
        {
            Id = 1,
            Status = EventStatus.Requested,
            NotificationSent = false,
            ContactEmail = "   "
        };

        var updated = new Event
        {
            Id = 1,
            Status = EventStatus.Planned,
            ContactEmail = "   "
        };

        // Act
        var decision = _service.ApplyChanges(existing, updated);

        // Assert
        Assert.False(decision.ShouldSendPlannedNotification);
        Assert.False(decision.PromoteToConfirmedAfterPlanned);
    }

    [Fact]
    public void ApplyChanges_TransitionToPlanned_WithNotificationAlreadySent_ReturnsSendPlannedFalse()
    {
        // Arrange
        var existing = new Event
        {
            Id = 1,
            Status = EventStatus.Requested,
            NotificationSent = true,
            ContactEmail = "contact@example.com"
        };

        var updated = new Event
        {
            Id = 1,
            Status = EventStatus.Planned,
            ContactEmail = "contact@example.com"
        };

        // Act
        var decision = _service.ApplyChanges(existing, updated);

        // Assert
        Assert.False(decision.ShouldSendPlannedNotification);
        Assert.False(decision.PromoteToConfirmedAfterPlanned);
    }

    [Fact]
    public void ApplyChanges_AlreadyPlanned_ReturnsSendPlannedFalse()
    {
        // Arrange
        var existing = new Event
        {
            Id = 1,
            Status = EventStatus.Planned,
            NotificationSent = false,
            ContactEmail = "contact@example.com"
        };

        var updated = new Event
        {
            Id = 1,
            Status = EventStatus.Planned,
            ContactEmail = "contact@example.com"
        };

        // Act
        var decision = _service.ApplyChanges(existing, updated);

        // Assert
        Assert.False(decision.ShouldSendPlannedNotification);
        Assert.False(decision.PromoteToConfirmedAfterPlanned);
    }

    [Fact]
    public void ApplyChanges_TransitionToSendInvoice_WithContactEmail_ReturnsSendInvoiceTrue()
    {
        // Arrange
        var existing = new Event
        {
            Id = 1,
            Status = EventStatus.Completed,
            ContactEmail = "contact@example.com"
        };

        var updated = new Event
        {
            Id = 1,
            Status = EventStatus.SendInvoice,
            ContactEmail = "contact@example.com"
        };

        // Act
        var decision = _service.ApplyChanges(existing, updated);

        // Assert
        Assert.False(decision.ShouldSendPlannedNotification);
        Assert.False(decision.PromoteToConfirmedAfterPlanned);
        Assert.True(decision.ShouldSendInvoiceNotification);
    }

    [Fact]
    public void ApplyChanges_TransitionToSendInvoice_WithoutContactEmail_ReturnsSendInvoiceFalse()
    {
        // Arrange
        var existing = new Event
        {
            Id = 1,
            Status = EventStatus.Completed,
            ContactEmail = null
        };

        var updated = new Event
        {
            Id = 1,
            Status = EventStatus.SendInvoice,
            ContactEmail = null
        };

        // Act
        var decision = _service.ApplyChanges(existing, updated);

        // Assert
        Assert.False(decision.ShouldSendInvoiceNotification);
    }

    [Fact]
    public void ApplyChanges_AlreadySendInvoice_ReturnsSendInvoiceFalse()
    {
        // Arrange
        var existing = new Event
        {
            Id = 1,
            Status = EventStatus.SendInvoice,
            ContactEmail = "contact@example.com"
        };

        var updated = new Event
        {
            Id = 1,
            Status = EventStatus.SendInvoice,
            ContactEmail = "contact@example.com"
        };

        // Act
        var decision = _service.ApplyChanges(existing, updated);

        // Assert
        Assert.False(decision.ShouldSendInvoiceNotification);
    }

    [Fact]
    public void ApplyChanges_BothPlannedAndInvoiceTransitions_ReturnsBothTrue()
    {
        // Arrange - This is an edge case where status transitions through multiple states
        var existing = new Event
        {
            Id = 1,
            Status = EventStatus.Requested,
            NotificationSent = false,
            ContactEmail = "contact@example.com"
        };

        var updated = new Event
        {
            Id = 1,
            Status = EventStatus.Planned,
            ContactEmail = "contact@example.com"
        };

        // Act - First transition to Planned
        var decision = _service.ApplyChanges(existing, updated);

        // Assert
        Assert.True(decision.ShouldSendPlannedNotification);
        Assert.True(decision.PromoteToConfirmedAfterPlanned);
        Assert.False(decision.ShouldSendInvoiceNotification);
    }

    [Fact]
    public void ApplyChanges_NoStatusChange_ReturnsAllFalse()
    {
        // Arrange
        var existing = new Event
        {
            Id = 1,
            Status = EventStatus.Confirmed,
            ContactEmail = "contact@example.com"
        };

        var updated = new Event
        {
            Id = 1,
            Status = EventStatus.Confirmed,
            ContactEmail = "contact@example.com"
        };

        // Act
        var decision = _service.ApplyChanges(existing, updated);

        // Assert
        Assert.False(decision.ShouldSendPlannedNotification);
        Assert.False(decision.PromoteToConfirmedAfterPlanned);
        Assert.False(decision.ShouldSendInvoiceNotification);
    }

    [Fact]
    public void ApplyChanges_TransitionFromConfirmedToPlanned_ReturnsSendPlannedTrue()
    {
        // Arrange - Testing backward status transition
        var existing = new Event
        {
            Id = 1,
            Status = EventStatus.Confirmed,
            NotificationSent = false,
            ContactEmail = "contact@example.com"
        };

        var updated = new Event
        {
            Id = 1,
            Status = EventStatus.Planned,
            ContactEmail = "contact@example.com"
        };

        // Act
        var decision = _service.ApplyChanges(existing, updated);

        // Assert - Notification is sent because status is transitioning TO Planned and notification wasn't sent yet
        Assert.True(decision.ShouldSendPlannedNotification);
        Assert.True(decision.PromoteToConfirmedAfterPlanned);
    }

    [Fact]
    public void ApplyChanges_UpdatesTimestamp_SetsUpdatedAt()
    {
        // Arrange
        var existing = new Event
        {
            Id = 1,
            Status = EventStatus.Requested,
            UpdatedAt = null
        };

        var updated = new Event
        {
            Id = 1,
            Status = EventStatus.Requested
        };

        var beforeUpdate = DateTime.UtcNow;

        // Act
        _service.ApplyChanges(existing, updated);

        // Assert
        Assert.NotNull(existing.UpdatedAt);
        Assert.True(existing.UpdatedAt >= beforeUpdate);
        Assert.True(existing.UpdatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void ApplyChanges_TransitionToCancelled_ReturnsAllFalse()
    {
        // Arrange
        var existing = new Event
        {
            Id = 1,
            Status = EventStatus.Requested,
            ContactEmail = "contact@example.com"
        };

        var updated = new Event
        {
            Id = 1,
            Status = EventStatus.Cancelled,
            ContactEmail = "contact@example.com"
        };

        // Act
        var decision = _service.ApplyChanges(existing, updated);

        // Assert
        Assert.False(decision.ShouldSendPlannedNotification);
        Assert.False(decision.PromoteToConfirmedAfterPlanned);
        Assert.False(decision.ShouldSendInvoiceNotification);
    }

    [Fact]
    public void ApplyChanges_TransitionFromRequestedToSendInvoice_ReturnsSendInvoiceTrue()
    {
        // Arrange - Direct jump to invoice status
        var existing = new Event
        {
            Id = 1,
            Status = EventStatus.Requested,
            ContactEmail = "contact@example.com"
        };

        var updated = new Event
        {
            Id = 1,
            Status = EventStatus.SendInvoice,
            ContactEmail = "contact@example.com"
        };

        // Act
        var decision = _service.ApplyChanges(existing, updated);

        // Assert
        Assert.False(decision.ShouldSendPlannedNotification);
        Assert.False(decision.PromoteToConfirmedAfterPlanned);
        Assert.True(decision.ShouldSendInvoiceNotification);
    }
}
