using Application.Common;
using Entities;

namespace Application.Queries.Shifts;

/// <summary>
/// Handler for getting shifts with an end date in the past.
/// </summary>
public class GetShiftsPastEndDateQueryHandler : IQueryHandler<GetShiftsPastEndDateQuery, List<Shift>>
{
    private readonly IShiftRepository _repository;

    public GetShiftsPastEndDateQueryHandler(IShiftRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Shift>> Handle(GetShiftsPastEndDateQuery query, CancellationToken cancellationToken = default)
    {
        var referenceDate = query.ReferenceDate ?? DateTime.UtcNow;
        var allShifts = await _repository.GetAllShiftsAsync();

        return allShifts
            .Where(shift => shift.EndTime < referenceDate)
            .OrderByDescending(shift => shift.EndTime)
            .ToList();
    }
}
