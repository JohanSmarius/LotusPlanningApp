using Application.Common;
using Entities;
using Microsoft.Extensions.Logging;

namespace Application.Commands.StaffAssignments;

/// <summary>
/// Handler for checking in staff for an assignment
/// </summary>
public class CheckInStaffCommandHandler : ICommandHandler<CheckInStaffCommand, StaffAssignment?>
{
    private readonly IStaffAssignmentRepository _repository;
    private readonly ILogger<CheckInStaffCommandHandler> _logger;

    public CheckInStaffCommandHandler(
        IStaffAssignmentRepository repository,
        ILogger<CheckInStaffCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<StaffAssignment?> Handle(CheckInStaffCommand command, CancellationToken cancellationToken = default)
    {
        var assignment = await _repository.CheckInStaffAsync(command.AssignmentId);
        
        if (assignment != null)
        {
            _logger.LogInformation("Staff checked in for assignment {AssignmentId}.", assignment.Id);
        }
        else
        {
            _logger.LogWarning("Failed to check in staff for assignment {AssignmentId}.", command.AssignmentId);
        }

        return assignment;
    }
}
