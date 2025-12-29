using Application;
using Application.Commands.StaffAssignments;
using Application.Common;
using LotusPlanningApp.Data;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Commands.StaffAssignments;

/// <summary>
/// Handles LinkUserToStaffByIdCommand
/// </summary>
public class LinkUserToStaffByIdCommandHandler : ICommandHandler<LinkUserToStaffByIdCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public LinkUserToStaffByIdCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> Handle(LinkUserToStaffByIdCommand command, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(command.UserId);
        if (user == null)
            return false;

        user.StaffId = command.StaffId;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }
}
