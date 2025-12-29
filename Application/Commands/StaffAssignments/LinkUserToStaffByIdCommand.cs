using Application.Common;

namespace Application.Commands.StaffAssignments;

/// <summary>
/// Command to link an ApplicationUser to a specific Staff member by ID
/// </summary>
public record LinkUserToStaffByIdCommand(string UserId, int StaffId) : ICommand<bool>;
