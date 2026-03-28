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
    /// Number of kilometres driven by the staff member from their home to the assignment location.
    /// </summary>
    public int? KmDriven { get; set; }

    /// <summary>
    /// Base64-encoded PNG image of the customer's signature.
    /// </summary>
    public string? CustomerSignature { get; set; }

    /// <summary>
    /// Name of the customer representative who signed for the completion of the assignment.
    /// </summary>
    [StringLength(100)]
    public string? CustomerSignedName { get; set; }

    /// <summary>
    /// The moment the customer signature was recorded.
    /// </summary>
    public DateTime? CustomerSignedAt { get; set; }

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
