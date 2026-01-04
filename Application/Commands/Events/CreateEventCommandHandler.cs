using Application.Common;
using Application.DataAdapters;
using Entities;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Events;

/// <summary>
/// Handler for creating a new event
/// </summary>
public class CreateEventCommandHandler : ICommandHandler<CreateEventCommand, EventDTO>
{
    private readonly IEventRepository _repository;
    private readonly ILogger<CreateEventCommandHandler> _logger;

    public CreateEventCommandHandler(
        IEventRepository repository,
        ILogger<CreateEventCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<EventDTO> Handle(CreateEventCommand command, CancellationToken cancellationToken = default)
    {
        var newEvent = command.EventData;

        // Validate dates
        if (newEvent.StartDate >= newEvent.EndDate)
        {
            throw new ApplicationLayerException("End date must be after start date.");
        }

        if (newEvent.StartDate <= DateTime.UtcNow)
        {
            throw new ApplicationLayerException("Start date must be in the future.");
        }

        var entity = newEvent.ToEntity();
        entity.Status = EventStatus.Requested;
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        // Create a shift to cover the entire event duration by default
        entity.Shifts.Add(new Shift
        {
            Name = "Default Shift",
            StartTime = newEvent.StartDate,
            EndTime = newEvent.EndDate,
            RequiredStaff = newEvent.RequiredStaffCount,
            Description = "Default shift covering the entire event duration",
            Status = ShiftStatus.Open,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        });

        var createdEvent = await _repository.CreateEventAsync(entity);

        _logger.LogInformation("Event {EventId} created successfully.", createdEvent.Id);

        return createdEvent.ToDTO();
    }
}
