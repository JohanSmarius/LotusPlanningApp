using Application.Common;
using Entities;

namespace Application.Commands.StaffAssignments;

/// <summary>
/// Command for a client to sign off on the actual hours worked and kilometers driven by a staff member
/// </summary>
public record SignOffStaffAssignmentCommand(
    int AssignmentId,
    decimal ActualHours,
    decimal KilometersDriven,
    byte[] ClientSignature) : ICommand<StaffAssignment?>;
