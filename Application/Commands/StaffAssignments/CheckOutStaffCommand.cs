using Application.Common;
using Entities;

namespace Application.Commands.StaffAssignments;

/// <summary>
/// Command to check out staff from an assignment
/// </summary>
public record CheckOutStaffCommand(int AssignmentId) : ICommand<StaffAssignment?>;
