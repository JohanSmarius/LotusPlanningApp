using Application.Common;
using Entities;

namespace Application.Queries.Shifts;

/// <summary>
/// Handler for getting a shift by ID
/// </summary>
public class GetShiftByIdQueryHandler : IQueryHandler<GetShiftByIdQuery, Shift?>
{
    private readonly IShiftRepository _repository;

    public GetShiftByIdQueryHandler(IShiftRepository repository)
    {
        _repository = repository;
    }

    public async Task<Shift?> Handle(GetShiftByIdQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.GetShiftByIdAsync(query.ShiftId);
    }
}
