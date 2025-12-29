using Application.Common;

namespace Application.Commands.Events;

/// <summary>
/// Command to delete an event
/// </summary>
public record DeleteEventCommand(int EventId) : ICommand<bool>;
