using Application.Common;
using Entities;

namespace Application.Queries.Events;

/// <summary>
/// Handler for getting upcoming events
/// </summary>
public class GetUpcomingEventsQueryHandler : IQueryHandler<GetUpcomingEventsQuery, List<Event>>
{
    private readonly IEventRepository _repository;

    public GetUpcomingEventsQueryHandler(IEventRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Event>> Handle(GetUpcomingEventsQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.GetUpcomingEventsAsync();
    }
}
