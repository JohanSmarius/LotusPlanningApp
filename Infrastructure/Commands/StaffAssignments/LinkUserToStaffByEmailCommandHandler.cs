using Application;
using Application.Commands.StaffAssignments;
using Application.Common;
using Entities;
using LotusPlanningApp.Data;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Commands.StaffAssignments;

/// <summary>
/// Handles LinkUserToStaffByEmailCommand
/// Attempts to link a user to an existing staff member by email.
/// If no staff member is found, creates a new one based on user details.
/// </summary>
public class LinkUserToStaffByEmailCommandHandler : ICommandHandler<LinkUserToStaffByEmailCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStaffRepository _staffRepository;

    public LinkUserToStaffByEmailCommandHandler(
        UserManager<ApplicationUser> userManager,
        IStaffRepository staffRepository)
    {
        _userManager = userManager;
        _staffRepository = staffRepository;
    }

    public async Task<bool> Handle(LinkUserToStaffByEmailCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.Email))
            return false;

        var user = await _userManager.FindByIdAsync(command.UserId);
        if (user == null)
            return false;

        // Try to find existing staff member by email
        var staff = await _staffRepository.GetStaffByEmailAsync(command.Email);

        // If not found, create a new staff member from user data
        if (staff == null)
        {
            staff = new Staff
            {
                FirstName = user.FirstName ?? "Unknown",
                LastName = user.LastName ?? "User",
                Email = command.Email,
                Role = StaffRole.LOTUS, // Default role
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            staff = await _staffRepository.CreateStaffAsync(staff);
        }

        // Link user to staff member
        user.StaffId = staff.Id;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }
}
