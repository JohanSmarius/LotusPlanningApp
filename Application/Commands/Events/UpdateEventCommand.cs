using Application.Common;
using Entities;

namespace Application.Commands.Events;

/// <summary>
/// Command to update an existing event
/// </summary>
public record UpdateEventCommand(Event UpdatedEvent) : ICommand<Event>;
