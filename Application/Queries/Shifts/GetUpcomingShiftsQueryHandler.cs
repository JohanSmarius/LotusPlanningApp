using Application.Common;
using Entities;

namespace Application.Queries.Shifts;

/// <summary>
/// Handler for getting upcoming shifts
/// </summary>
public class GetUpcomingShiftsQueryHandler : IQueryHandler<GetUpcomingShiftsQuery, List<Shift>>
{
    private readonly IShiftRepository _repository;

    public GetUpcomingShiftsQueryHandler(IShiftRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Shift>> Handle(GetUpcomingShiftsQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.GetUpcomingShiftsAsync();
    }
}
