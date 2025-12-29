using Application.Common;

namespace Application.Commands.StaffAssignments;

/// <summary>
/// Command to delete a staff assignment
/// </summary>
public record DeleteStaffAssignmentCommand(int AssignmentId) : ICommand<bool>;
