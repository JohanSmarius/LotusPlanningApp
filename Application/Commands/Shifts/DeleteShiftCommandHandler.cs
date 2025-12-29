using Application.Common;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Shifts;

/// <summary>
/// Handler for deleting a shift
/// </summary>
public class DeleteShiftCommandHandler : ICommandHandler<DeleteShiftCommand, bool>
{
    private readonly IShiftRepository _repository;
    private readonly ILogger<DeleteShiftCommandHandler> _logger;

    public DeleteShiftCommandHandler(
        IShiftRepository repository,
        ILogger<DeleteShiftCommandHandler> logger)
    {
        _repository = repository;
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
        
        return true;
    }
}
