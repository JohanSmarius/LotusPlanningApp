using Application.Common;
using Entities;
using Microsoft.Extensions.Logging;

namespace Application.Commands.StaffAssignments;

/// <summary>
/// Handler for updating the number of kilometres ridden during an assignment.
/// </summary>
public class UpdateRiddenKmCommandHandler : ICommandHandler<UpdateRiddenKmCommand, StaffAssignment?>
{
    private readonly IStaffAssignmentRepository _repository;
    private readonly ILogger<UpdateRiddenKmCommandHandler> _logger;

    public UpdateRiddenKmCommandHandler(
        IStaffAssignmentRepository repository,
        ILogger<UpdateRiddenKmCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<StaffAssignment?> Handle(UpdateRiddenKmCommand command, CancellationToken cancellationToken = default)
    {
        if (command.RiddenKm < 0 || command.RiddenKm > 10000)
        {
            _logger.LogWarning("Invalid ridden km value {RiddenKm} for assignment {AssignmentId}.", command.RiddenKm, command.AssignmentId);
            return null;
        }

        // When a specific staff member is making the request, verify they own the assignment.
        if (command.RequestingStaffId.HasValue)
        {
            var existing = await _repository.GetAssignmentByIdAsync(command.AssignmentId);
            if (existing == null || existing.StaffId != command.RequestingStaffId.Value)
            {
                _logger.LogWarning("Staff {StaffId} is not authorised to update ridden km for assignment {AssignmentId}.", command.RequestingStaffId, command.AssignmentId);
                return null;
            }
        }

        var assignment = await _repository.UpdateRiddenKmAsync(command.AssignmentId, command.RiddenKm);

        if (assignment != null)
        {
            _logger.LogInformation("Ridden km updated to {RiddenKm} for assignment {AssignmentId}.", command.RiddenKm, assignment.Id);
        }
        else
        {
            _logger.LogWarning("Failed to update ridden km for assignment {AssignmentId}.", command.AssignmentId);
        }

        return assignment;
    }
}
