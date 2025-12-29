using Application.Common;
using Entities;

namespace Application.Queries.Events;

/// <summary>
/// Handler for getting an event by ID
/// </summary>
public class GetEventByIdQueryHandler : IQueryHandler<GetEventByIdQuery, Event?>
{
    private readonly IEventRepository _repository;

    public GetEventByIdQueryHandler(IEventRepository repository)
    {
        _repository = repository;
    }

    public async Task<Event?> Handle(GetEventByIdQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.GetEventByIdAsync(query.EventId);
    }
}
