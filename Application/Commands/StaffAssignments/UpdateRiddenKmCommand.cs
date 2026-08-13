using Application.Common;
using Entities;

namespace Application.Commands.StaffAssignments;

/// <summary>
/// Command to update the number of kilometres ridden during an assignment.
/// When <paramref name="RequestingStaffId"/> is provided, the handler verifies that
/// it matches the assignment's staff before updating (non-admin callers should supply this).
/// Admins may pass <c>null</c> to skip the ownership check.
/// </summary>
public record UpdateRiddenKmCommand(int AssignmentId, int RiddenKm, int? RequestingStaffId = null) : ICommand<StaffAssignment?>;
