using Application.Common;
using Microsoft.Extensions.Logging;

namespace Application.Commands.StaffAssignments;

/// <summary>
/// Handler for deleting staff assignments
/// </summary>
public class DeleteStaffAssignmentCommandHandler : ICommandHandler<DeleteStaffAssignmentCommand, bool>
{
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<DeleteStaffAssignmentCommandHandler> _logger;

    public DeleteStaffAssignmentCommandHandler(
        IStaffAssignmentRepository staffAssignmentRepository,
        IEmailService emailService,
        ILogger<DeleteStaffAssignmentCommandHandler> logger)
    {
        _staffAssignmentRepository = staffAssignmentRepository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteStaffAssignmentCommand command, CancellationToken cancellationToken = default)
    {
        // Load the assignment with related data before deleting it so we can notify the staff member.
        var assignment = await _staffAssignmentRepository.GetAssignmentByIdAsync(command.AssignmentId);

        await _staffAssignmentRepository.DeleteAssignmentAsync(command.AssignmentId);

        if (assignment?.Staff != null && assignment.Shift?.Event != null)
        {
            try
            {
                await _emailService.SendStaffAssignmentDeletionNotificationAsync(
                    assignment.Staff,
                    assignment.Shift,
                    assignment.Shift.Event);

                _logger.LogInformation("Assignment deletion notification email sent to {Email} for shift {ShiftName}",
                    assignment.Staff.Email, assignment.Shift.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send assignment deletion notification email to {Email} for shift {ShiftName}",
                    assignment.Staff?.Email, assignment.Shift?.Name);
                // Don't throw - we don't want email failures to prevent assignment deletion
            }
        }

        return true;
    }
}
