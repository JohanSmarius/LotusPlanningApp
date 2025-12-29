using Application.Common;

namespace Application.Commands.StaffAssignments;

/// <summary>
/// Handler for deleting staff assignments
/// </summary>
public class DeleteStaffAssignmentCommandHandler : ICommandHandler<DeleteStaffAssignmentCommand, bool>
{
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;

    public DeleteStaffAssignmentCommandHandler(IStaffAssignmentRepository staffAssignmentRepository)
    {
        _staffAssignmentRepository = staffAssignmentRepository;
    }

    public async Task<bool> Handle(DeleteStaffAssignmentCommand command, CancellationToken cancellationToken = default)
    {
        await _staffAssignmentRepository.DeleteAssignmentAsync(command.AssignmentId);
        return true;
    }
}
