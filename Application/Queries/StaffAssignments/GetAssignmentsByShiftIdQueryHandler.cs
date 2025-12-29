using Application.Common;
using Entities;

namespace Application.Queries.StaffAssignments;

/// <summary>
/// Handler for getting assignments by shift ID
/// </summary>
public class GetAssignmentsByShiftIdQueryHandler : IQueryHandler<GetAssignmentsByShiftIdQuery, List<StaffAssignment>>
{
    private readonly IStaffAssignmentRepository _repository;

    public GetAssignmentsByShiftIdQueryHandler(IStaffAssignmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<StaffAssignment>> Handle(GetAssignmentsByShiftIdQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.GetAssignmentsByShiftIdAsync(query.ShiftId);
    }
}
