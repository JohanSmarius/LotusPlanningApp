using Application.Common;
using Entities;

namespace Application.Queries.Shifts;

/// <summary>
/// Handler for getting all upcoming shifts with status Open
/// </summary>
public class GetOpenShiftsQueryHandler : IQueryHandler<GetOpenShiftsQuery, List<Shift>>
{
    private readonly IShiftRepository _repository;

    public GetOpenShiftsQueryHandler(IShiftRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Shift>> Handle(GetOpenShiftsQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.GetOpenShiftsAsync();
    }
}
