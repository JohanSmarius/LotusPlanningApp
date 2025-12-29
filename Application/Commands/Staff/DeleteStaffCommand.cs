using Application.Common;

namespace Application.Commands.Staff;

/// <summary>
/// Command to delete a staff member
/// </summary>
public record DeleteStaffCommand(int StaffId) : ICommand<bool>;
