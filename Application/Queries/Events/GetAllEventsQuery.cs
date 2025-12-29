using Application.Common;
using Entities;

namespace Application.Queries.Events;

/// <summary>
/// Query to get all events
/// </summary>
public record GetAllEventsQuery : IQuery<List<Event>>;
