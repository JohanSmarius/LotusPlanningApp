using Application.Common;
using Entities;
using Microsoft.Extensions.Logging;

namespace Application.Commands.StaffAssignments;

/// <summary>
/// Handler for creating a new staff assignment
/// </summary>
public class CreateStaffAssignmentCommandHandler : ICommandHandler<CreateStaffAssignmentCommand, StaffAssignment>
{
    private readonly IStaffAssignmentRepository _repository;
    private readonly IShiftRepository _shiftRepository;
    private readonly ILogger<CreateStaffAssignmentCommandHandler> _logger;

    public CreateStaffAssignmentCommandHandler(
        IStaffAssignmentRepository repository,
        IShiftRepository shiftRepository,
        ILogger<CreateStaffAssignmentCommandHandler> logger)
    {
        _repository = repository;
        _shiftRepository = shiftRepository;
        _logger = logger;
    }

    public async Task<StaffAssignment> Handle(CreateStaffAssignmentCommand command, CancellationToken cancellationToken = default)
    {
        var assignment = command.Assignment;
        
        // Get the shift to validate time range
        var shift = await _shiftRepository.GetShiftByIdAsync(assignment.ShiftId);
        if (shift == null)
        {
            throw new ApplicationLayerException($"Shift {assignment.ShiftId} not found.");
        }

        // Check if staff is available for this time period
        if (!await _repository.IsStaffAvailableAsync(assignment.StaffId, shift.StartTime, shift.EndTime))
        {
            throw new ApplicationLayerException("Staff member is not available for this shift.");
        }

        assignment.AssignedAt = DateTime.UtcNow;
        assignment.UpdatedAt = DateTime.UtcNow;

        var createdAssignment = await _repository.CreateAssignmentAsync(assignment);
        _logger.LogInformation("Staff assignment {AssignmentId} created successfully.", createdAssignment.Id);

        return createdAssignment;
    }
}
