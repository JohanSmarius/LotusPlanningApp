using Application.Common;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Shifts;

/// <summary>
/// Handler for deleting a shift
/// </summary>
public class DeleteShiftCommandHandler : ICommandHandler<DeleteShiftCommand, bool>
{
    private readonly IShiftRepository _repository;
    private readonly IEmailService _emailService;
    private readonly ILogger<DeleteShiftCommandHandler> _logger;

    public DeleteShiftCommandHandler(
        IShiftRepository repository,
        IEmailService emailService,
        ILogger<DeleteShiftCommandHandler> logger)
    {
        _repository = repository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteShiftCommand command, CancellationToken cancellationToken = default)
    {
        var shiftId = command.ShiftId;
        
        var existingShift = await _repository.GetShiftByIdAsync(shiftId);
        if (existingShift == null)
        {
            _logger.LogWarning("Attempted to delete non-existent shift {ShiftId}", shiftId);
            return false;
        }

        await _repository.DeleteShiftAsync(shiftId);
        _logger.LogInformation("Shift {ShiftId} deleted successfully.", shiftId);

        if (existingShift.Event != null && existingShift.StaffAssignments != null && existingShift.StaffAssignments.Count > 0)
        {
            foreach (var assignment in existingShift.StaffAssignments)
            {
                if (assignment.Staff != null)
                {
                    try
                    {
                        await _emailService.SendStaffAssignmentDeletionNotificationAsync(
                            assignment.Staff,
                            existingShift,
                            existingShift.Event);

                        _logger.LogInformation("Assignment deletion notification email sent to {Email} for deleted shift {ShiftName}",
                            assignment.Staff.Email, existingShift.Name);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send assignment deletion notification email to {Email} for deleted shift {ShiftName}",
                            assignment.Staff?.Email, existingShift.Name);
                        // Don't throw - we don't want email failures to prevent shift deletion
                    }
                }
            }
        }
        
        return true;
    }
}
