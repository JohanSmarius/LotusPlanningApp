using Application.Common;
using Entities;

namespace Application.Queries.Events;

/// <summary>
/// Query to get upcoming events
/// </summary>
public record GetUpcomingEventsQuery : IQuery<List<Event>>;
