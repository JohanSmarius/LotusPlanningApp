using Application.Commands.Events;
using Entities;

namespace Application;

/// <summary>
/// Legacy wrapper for event commands - maintained for backward compatibility
/// </summary>
public class EventService : IEventService
{
    private readonly UpdateEventCommandHandler _updateHandler;
        
    public EventService(UpdateEventCommandHandler updateHandler)
    {
        _updateHandler = updateHandler;
    }

    public async Task<Event> UpdateEventAsync(Event updated)
    {
        var command = new UpdateEventCommand(updated);
        return await _updateHandler.Handle(command);
    }
}