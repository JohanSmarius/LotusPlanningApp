using Application.Common;
using Entities;

namespace Application.Queries.Events;

/// <summary>
/// Query to get events by date range
/// </summary>
public record GetEventsByDateRangeQuery(DateTime StartDate, DateTime EndDate) : IQuery<List<Event>>;
