using Application.Common;
using Entities;

namespace Application.Commands.StaffAssignments;

/// <summary>
/// Command to create a new staff assignment
/// </summary>
public record CreateStaffAssignmentCommand(StaffAssignment Assignment, bool SendNotification = true) : ICommand<StaffAssignment>;
