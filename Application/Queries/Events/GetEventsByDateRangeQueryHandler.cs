using Application.Common;
using Entities;

namespace Application.Queries.Events;

/// <summary>
/// Handler for getting events by date range
/// </summary>
public class GetEventsByDateRangeQueryHandler : IQueryHandler<GetEventsByDateRangeQuery, List<Event>>
{
    private readonly IEventRepository _repository;

    public GetEventsByDateRangeQueryHandler(IEventRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Event>> Handle(GetEventsByDateRangeQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.GetEventsByDateRangeAsync(query.StartDate, query.EndDate);
    }
}
