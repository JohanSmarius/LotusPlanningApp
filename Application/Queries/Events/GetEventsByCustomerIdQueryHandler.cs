using Application.Common;
using Application.DataAdapters;
using Application.Queries.Events;

namespace Application.Queries.Events;

/// <summary>
/// Handler for getting events by customer ID
/// </summary>
public class GetEventsByCustomerIdQueryHandler : IQueryHandler<GetEventsByCustomerIdQuery, List<EventDTO>>
{
    private readonly IEventRepository _repository;

    public GetEventsByCustomerIdQueryHandler(IEventRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<EventDTO>> Handle(GetEventsByCustomerIdQuery query, CancellationToken cancellationToken = default)
    {
        var allEvents = await _repository.GetAllEventsAsync();
        var customerEvents = allEvents.Where(e => e.CustomerId == query.CustomerId).ToList();
        return customerEvents.Select(e => e.ToDTO()).ToList();
    }
}
