using Application.Common;
using Entities;

namespace Application.Queries.Events;

/// <summary>
/// Query to get an event by ID
/// </summary>
public record GetEventByIdQuery(int EventId) : IQuery<Event?>;
