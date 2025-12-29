using Application.Common;
using Entities;

namespace Application.Queries.Events;

/// <summary>
/// Handler for getting all events
/// </summary>
public class GetAllEventsQueryHandler : IQueryHandler<GetAllEventsQuery, List<Event>>
{
    private readonly IEventRepository _repository;

    public GetAllEventsQueryHandler(IEventRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Event>> Handle(GetAllEventsQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllEventsAsync();
    }
}
