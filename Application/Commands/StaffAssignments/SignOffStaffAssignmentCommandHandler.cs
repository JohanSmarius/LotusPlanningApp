using Application.Common;
using Entities;
using Microsoft.Extensions.Logging;

namespace Application.Commands.StaffAssignments;

/// <summary>
/// Handler for the client sign-off command
/// </summary>
public class SignOffStaffAssignmentCommandHandler : ICommandHandler<SignOffStaffAssignmentCommand, StaffAssignment?>
{
    private readonly IStaffAssignmentRepository _repository;
    private readonly ILogger<SignOffStaffAssignmentCommandHandler> _logger;

    public SignOffStaffAssignmentCommandHandler(
        IStaffAssignmentRepository repository,
        ILogger<SignOffStaffAssignmentCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<StaffAssignment?> Handle(SignOffStaffAssignmentCommand command, CancellationToken cancellationToken = default)
    {
        var assignment = await _repository.SignOffAssignmentAsync(
            command.AssignmentId,
            command.ActualHours,
            command.KilometersDriven,
            command.ClientSignature);

        if (assignment != null)
        {
            _logger.LogInformation(
                "Client signed off assignment {AssignmentId}: {Hours} hours, {Km} km.",
                assignment.Id, command.ActualHours, command.KilometersDriven);
        }
        else
        {
            _logger.LogWarning("Failed to sign off assignment {AssignmentId}.", command.AssignmentId);
        }

        return assignment;
    }
}
