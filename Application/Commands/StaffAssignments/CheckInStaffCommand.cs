using Application.Common;
using Entities;

namespace Application.Commands.StaffAssignments;

/// <summary>
/// Command to check in staff for an assignment
/// </summary>
public record CheckInStaffCommand(int AssignmentId) : ICommand<StaffAssignment?>;
