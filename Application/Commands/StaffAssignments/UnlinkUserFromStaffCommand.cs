using Application.Common;

namespace Application.Commands.StaffAssignments;

/// <summary>
/// Command to unlink an ApplicationUser from their Staff member
/// </summary>
public record UnlinkUserFromStaffCommand(string UserId) : ICommand<bool>;
