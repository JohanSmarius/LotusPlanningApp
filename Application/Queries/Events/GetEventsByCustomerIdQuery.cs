using Application.Common;
using Application.DataAdapters;

namespace Application.Queries.Events;

/// <summary>
/// Query to get events by customer ID
/// </summary>
public record GetEventsByCustomerIdQuery(int CustomerId) : IQuery<List<EventDTO>>;
