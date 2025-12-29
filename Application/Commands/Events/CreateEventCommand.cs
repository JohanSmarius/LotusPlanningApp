using Application.Common;
using Application.DataAdapters;

namespace Application.Commands.Events;

/// <summary>
/// Command to create a new event
/// </summary>
public record CreateEventCommand(EventDTO EventData) : ICommand<EventDTO>;
