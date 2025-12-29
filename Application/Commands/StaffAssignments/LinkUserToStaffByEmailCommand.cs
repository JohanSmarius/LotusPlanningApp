using Application.Common;

namespace Application.Commands.StaffAssignments;

/// <summary>
/// Command to link an ApplicationUser to a Staff member by matching email addresses
/// </summary>
public record LinkUserToStaffByEmailCommand(string UserId, string Email) : ICommand<bool>;
