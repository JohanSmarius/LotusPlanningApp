using Application.Common;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Events;

/// <summary>
/// Handler for deleting an event
/// </summary>
public class DeleteEventCommandHandler : ICommandHandler<DeleteEventCommand, bool>
{
    private readonly IEventRepository _repository;
    private readonly ILogger<DeleteEventCommandHandler> _logger;

    public DeleteEventCommandHandler(
        IEventRepository repository,
        ILogger<DeleteEventCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteEventCommand command, CancellationToken cancellationToken = default)
    {
        var eventId = command.EventId;
        
        var existingEvent = await _repository.GetEventByIdAsync(eventId);
        if (existingEvent == null)
        {
            _logger.LogWarning("Attempted to delete non-existent event {EventId}", eventId);
            return false;
        }

        await _repository.DeleteEventAsync(eventId);
        _logger.LogInformation("Event {EventId} deleted successfully.", eventId);
        
        return true;
    }
}
