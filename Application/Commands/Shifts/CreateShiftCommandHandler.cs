using Application.Common;
using Entities;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Shifts;

/// <summary>
/// Handler for creating a new shift
/// </summary>
public class CreateShiftCommandHandler : ICommandHandler<CreateShiftCommand, Shift>
{
    private readonly IShiftRepository _repository;
    private readonly ILogger<CreateShiftCommandHandler> _logger;

    public CreateShiftCommandHandler(
        IShiftRepository repository,
        ILogger<CreateShiftCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Shift> Handle(CreateShiftCommand command, CancellationToken cancellationToken = default)
    {
        var shift = command.Shift;
        
        // Validate shift times
        if (shift.StartTime >= shift.EndTime)
        {
            throw new ApplicationLayerException("Shift end time must be after start time.");
        }

        shift.CreatedAt = DateTime.UtcNow;
        shift.UpdatedAt = DateTime.UtcNow;

        var createdShift = await _repository.CreateShiftAsync(shift);
        _logger.LogInformation("Shift {ShiftId} created successfully.", createdShift.Id);

        return createdShift;
    }
}
