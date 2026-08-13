using System.ComponentModel.DataAnnotations;

namespace Entities;

/// <summary>
/// Represents assignment of staff to a shift
/// </summary>
public class StaffAssignment
{
    public int Id { get; set; }

    [Required]
    public int ShiftId { get; set; }

    [Required]
    public int StaffId { get; set; }

    [Required]
    public StaffRole Role { get; set; }

    public AssignmentStatus Status { get; set; } = AssignmentStatus.Assigned;

    [StringLength(300)]
    public string? Notes { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Actual number of hours worked, as confirmed by the client during sign-off
    /// </summary>
    public decimal? ActualHours { get; set; }

    /// <summary>
    /// Kilometers driven by the staff member, as confirmed by the client during sign-off
    /// </summary>
    public decimal? KilometersDriven { get; set; }

    /// <summary>
    /// Base-64 encoded PNG of the client's signature
    /// </summary>
    public string? ClientSignature { get; set; }

    /// <summary>
    /// Date and time when the client signed off on this assignment
    /// </summary>
    public DateTime? SignedOffAt { get; set; }

    // Navigation properties
    public Shift Shift { get; set; } = null!;
    public Staff Staff { get; set; } = null!;
}

/// <summary>
/// Status of a staff assignment
/// </summary>
public enum AssignmentStatus
{
    Assigned,
    Confirmed,
    CheckedIn,
    CheckedOut,
    NoShow,
    Cancelled
}

/// <summary>
/// Role of a staff member
/// </summary>
public enum StaffRole
{
    LOTUS,
    Coordinator
}
