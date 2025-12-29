using Application.Common;
using Entities;
using Microsoft.Extensions.Logging;

namespace Application.Commands.StaffAssignments;

/// <summary>
/// Handler for checking out staff from an assignment
/// </summary>
public class CheckOutStaffCommandHandler : ICommandHandler<CheckOutStaffCommand, StaffAssignment?>
{
    private readonly IStaffAssignmentRepository _repository;
    private readonly ILogger<CheckOutStaffCommandHandler> _logger;

    public CheckOutStaffCommandHandler(
        IStaffAssignmentRepository repository,
        ILogger<CheckOutStaffCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<StaffAssignment?> Handle(CheckOutStaffCommand command, CancellationToken cancellationToken = default)
    {
        var assignment = await _repository.CheckOutStaffAsync(command.AssignmentId);
        
        if (assignment != null)
        {
            _logger.LogInformation("Staff checked out from assignment {AssignmentId}.", assignment.Id);
        }
        else
        {
            _logger.LogWarning("Failed to check out staff from assignment {AssignmentId}.", command.AssignmentId);
        }

        return assignment;
    }
}
