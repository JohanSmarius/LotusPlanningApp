using Application.Common;
using Entities;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Shifts;

/// <summary>
/// Handler for updating an existing shift
/// </summary>
public class UpdateShiftCommandHandler : ICommandHandler<UpdateShiftCommand, Shift>
{
    private readonly IShiftRepository _repository;
    private readonly ILogger<UpdateShiftCommandHandler> _logger;

    public UpdateShiftCommandHandler(
        IShiftRepository repository,
        ILogger<UpdateShiftCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Shift> Handle(UpdateShiftCommand command, CancellationToken cancellationToken = default)
    {
        var shift = command.Shift;
        
        // Validate shift times
        if (shift.StartTime >= shift.EndTime)
        {
            throw new ApplicationLayerException("Shift end time must be after start time.");
        }

        shift.UpdatedAt = DateTime.UtcNow;

        var updatedShift = await _repository.UpdateShiftAsync(shift);
        _logger.LogInformation("Shift {ShiftId} updated successfully.", updatedShift.Id);

        return updatedShift;
    }
}
