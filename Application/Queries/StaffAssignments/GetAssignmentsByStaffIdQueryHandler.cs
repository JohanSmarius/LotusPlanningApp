using Application.Common;
using Entities;

namespace Application.Queries.StaffAssignments;

/// <summary>
/// Handler for getting assignments by staff ID
/// </summary>
public class GetAssignmentsByStaffIdQueryHandler : IQueryHandler<GetAssignmentsByStaffIdQuery, List<StaffAssignment>>
{
    private readonly IStaffAssignmentRepository _repository;

    public GetAssignmentsByStaffIdQueryHandler(IStaffAssignmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<StaffAssignment>> Handle(GetAssignmentsByStaffIdQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.GetAssignmentsByStaffIdAsync(query.StaffId);
    }
}
