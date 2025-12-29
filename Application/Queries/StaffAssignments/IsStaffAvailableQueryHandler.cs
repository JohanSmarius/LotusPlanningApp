using Application.Common;

namespace Application.Queries.StaffAssignments;

/// <summary>
/// Handler for checking staff availability during a time period
/// </summary>
public class IsStaffAvailableQueryHandler : IQueryHandler<IsStaffAvailableQuery, bool>
{
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;

    public IsStaffAvailableQueryHandler(IStaffAssignmentRepository staffAssignmentRepository)
    {
        _staffAssignmentRepository = staffAssignmentRepository;
    }

    public async Task<bool> Handle(IsStaffAvailableQuery query, CancellationToken cancellationToken = default)
    {
        return await _staffAssignmentRepository.IsStaffAvailableAsync(
            query.StaffId, 
            query.StartTime, 
            query.EndTime);
    }
}
