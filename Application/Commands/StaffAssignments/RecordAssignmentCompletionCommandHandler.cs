using Application.Common;
using Entities;
using Microsoft.Extensions.Logging;

namespace Application.Commands.StaffAssignments;

/// <summary>
/// Handler for recording completion details of a staff assignment.
/// </summary>
public class RecordAssignmentCompletionCommandHandler : ICommandHandler<RecordAssignmentCompletionCommand, StaffAssignment?>
{
    private readonly IStaffAssignmentRepository _repository;
    private readonly ILogger<RecordAssignmentCompletionCommandHandler> _logger;

    public RecordAssignmentCompletionCommandHandler(
        IStaffAssignmentRepository repository,
        ILogger<RecordAssignmentCompletionCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the <see cref="RecordAssignmentCompletionCommand"/> by persisting km driven and the customer signature.
    /// </summary>
    public async Task<StaffAssignment?> Handle(RecordAssignmentCompletionCommand command, CancellationToken cancellationToken = default)
    {
        var assignment = await _repository.RecordAssignmentCompletionAsync(
            command.AssignmentId,
            command.KmDriven,
            command.CustomerSignature,
            command.CustomerSignedName);

        if (assignment != null)
        {
            _logger.LogInformation(
                "Completion details recorded for assignment {AssignmentId}: {KmDriven} km, signature provided: {HasSignature}.",
                assignment.Id,
                command.KmDriven,
                !string.IsNullOrWhiteSpace(command.CustomerSignature));
        }
        else
        {
            _logger.LogWarning("Assignment {AssignmentId} not found when recording completion.", command.AssignmentId);
        }

        return assignment;
    }
}
