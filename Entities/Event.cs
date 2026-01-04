using System.ComponentModel.DataAnnotations;

namespace Entities;

/// <summary>
/// Represents a medical first aid event
/// </summary>
public class Event
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    [StringLength(200)]
    public string Location { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public EventStatus Status { get; set; } = EventStatus.Requested;

    [StringLength(100)]
    public string? ContactPerson { get; set; }

    [StringLength(20)]
    public string? ContactPhone { get; set; }

    [EmailAddress]
    [StringLength(255)]
    public string? ContactEmail { get; set; }

    public bool NotificationSent { get; set; } = false;

    [Range(1, 50)]
    public int RequiredStaffCount { get; set; } = 1;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Reference to the customer who created this event
    /// </summary>
    public int? CustomerId { get; set; }

    /// <summary>
    /// Indicates if a cancellation has been requested for this event
    /// </summary>
    public bool CancellationRequested { get; set; } = false;

    /// <summary>
    /// Date when cancellation was requested
    /// </summary>
    public DateTime? CancellationRequestedAt { get; set; }

    /// <summary>
    /// Reason for cancellation request
    /// </summary>
    [StringLength(500)]
    public string? CancellationReason { get; set; }

    // Navigation properties
    public Customer? Customer { get; set; }
    public List<Shift> Shifts { get; set; } = new();
}

/// <summary>
/// Status of an event
/// </summary>
public enum EventStatus
{
    Requested,
    Planned,
    Confirmed,
    Active,
    Completed,
    SendInvoice,
    Cancelled
}
