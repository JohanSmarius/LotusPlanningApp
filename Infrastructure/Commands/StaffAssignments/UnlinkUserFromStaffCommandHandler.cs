using Application;
using Application.Commands.StaffAssignments;
using Application.Common;
using LotusPlanningApp.Data;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Commands.StaffAssignments;

/// <summary>
/// Handles UnlinkUserFromStaffCommand
/// </summary>
public class UnlinkUserFromStaffCommandHandler : ICommandHandler<UnlinkUserFromStaffCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UnlinkUserFromStaffCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> Handle(UnlinkUserFromStaffCommand command, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(command.UserId);
        if (user == null)
            return false;

        user.StaffId = null;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }
}
