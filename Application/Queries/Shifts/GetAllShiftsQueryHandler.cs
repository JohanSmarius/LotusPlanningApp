using Application.Common;
using Entities;

namespace Application.Queries.Shifts;

/// <summary>
/// Handler for getting all shifts
/// </summary>
public class GetAllShiftsQueryHandler : IQueryHandler<GetAllShiftsQuery, List<Shift>>
{
    private readonly IShiftRepository _repository;

    public GetAllShiftsQueryHandler(IShiftRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Shift>> Handle(GetAllShiftsQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllShiftsAsync();
    }
}
