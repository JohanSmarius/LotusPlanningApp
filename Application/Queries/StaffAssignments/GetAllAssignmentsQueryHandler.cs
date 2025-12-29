using Application.Common;
using Entities;

namespace Application.Queries.StaffAssignments;

/// <summary>
/// Handler for getting all staff assignments
/// </summary>
public class GetAllAssignmentsQueryHandler : IQueryHandler<GetAllAssignmentsQuery, List<StaffAssignment>>
{
    private readonly IStaffAssignmentRepository _repository;

    public GetAllAssignmentsQueryHandler(IStaffAssignmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<StaffAssignment>> Handle(GetAllAssignmentsQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllAssignmentsAsync();
    }
}
