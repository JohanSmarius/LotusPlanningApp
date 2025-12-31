using Application.Common;
using Entities;

namespace Application.Queries.StaffAssignments;

/// <summary>
/// Handler for getting assignments by staff ID where the event status is Confirmed
/// </summary>
public class GetConfirmedAssignmentsByStaffIdQueryHandler : IQueryHandler<GetConfirmedAssignmentsByStaffIdQuery, List<StaffAssignment>>
{
    private readonly IStaffAssignmentRepository _repository;

    public GetConfirmedAssignmentsByStaffIdQueryHandler(IStaffAssignmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<StaffAssignment>> Handle(GetConfirmedAssignmentsByStaffIdQuery query, CancellationToken cancellationToken = default)
    {
        // Get all assignments for the staff member
        var allAssignments = await _repository.GetAssignmentsByStaffIdAsync(query.StaffId);
        
        // Filter to show only assignments with confirmed events and within the date range
        return allAssignments
            .Where(a => a.Shift?.Event?.Status == EventStatus.Confirmed && 
                       a.Shift.StartTime >= query.CutoffDate)
            .ToList();
    }
}
