using Application.Commands.Customers;
using Application.Common;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Customers;

/// <summary>
/// Handler for requesting event cancellation
/// </summary>
public class RequestEventCancellationCommandHandler : ICommandHandler<RequestEventCancellationCommand, bool>
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<RequestEventCancellationCommandHandler> _logger;

    public RequestEventCancellationCommandHandler(
        IEventRepository eventRepository,
        ILogger<RequestEventCancellationCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(RequestEventCancellationCommand command, CancellationToken cancellationToken = default)
    {
        var eventEntity = await _eventRepository.GetEventByIdAsync(command.EventId);
        if (eventEntity == null)
        {
            throw new ApplicationLayerException($"Event with ID {command.EventId} not found.");
        }

        if (eventEntity.CancellationRequested)
        {
            throw new ApplicationLayerException("Cancellation has already been requested for this event.");
        }

        eventEntity.CancellationRequested = true;
        eventEntity.CancellationRequestedAt = DateTime.UtcNow;
        eventEntity.CancellationReason = command.Reason;
        eventEntity.UpdatedAt = DateTime.UtcNow;

        await _eventRepository.UpdateEventAsync(eventEntity);

        _logger.LogInformation("Cancellation requested for Event {EventId}: {Reason}", command.EventId, command.Reason);

        return true;
    }
}
