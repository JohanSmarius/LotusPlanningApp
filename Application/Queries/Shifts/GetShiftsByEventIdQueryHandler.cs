using Application.Common;
using Entities;

namespace Application.Queries.Shifts;

/// <summary>
/// Handler for getting shifts by event ID
/// </summary>
public class GetShiftsByEventIdQueryHandler : IQueryHandler<GetShiftsByEventIdQuery, List<Shift>>
{
    private readonly IShiftRepository _repository;

    public GetShiftsByEventIdQueryHandler(IShiftRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Shift>> Handle(GetShiftsByEventIdQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.GetShiftsByEventIdAsync(query.EventId);
    }
}
