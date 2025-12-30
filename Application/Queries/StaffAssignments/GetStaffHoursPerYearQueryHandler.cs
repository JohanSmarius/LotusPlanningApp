using Application.Common;
using Application.DataAdapters;

namespace Application.Queries.StaffAssignments;

/// <summary>
/// Handler for getting staff hours per year
/// </summary>
public class GetStaffHoursPerYearQueryHandler : IQueryHandler<GetStaffHoursPerYearQuery, List<StaffHoursDTO>>
{
    private readonly IStaffAssignmentRepository _assignmentRepository;
    private readonly IStaffRepository _staffRepository;

    public GetStaffHoursPerYearQueryHandler(
        IStaffAssignmentRepository assignmentRepository,
        IStaffRepository staffRepository)
    {
        _assignmentRepository = assignmentRepository;
        _staffRepository = staffRepository;
    }

    public async Task<List<StaffHoursDTO>> Handle(GetStaffHoursPerYearQuery query, CancellationToken cancellationToken = default)
    {
        // Get all assignments
        var allAssignments = await _assignmentRepository.GetAllAssignmentsAsync();
        
        // Filter assignments for the specified year based on shift start time
        var yearAssignments = allAssignments
            .Where(a => a.Shift != null && 
                       a.Shift.StartTime.Year == query.Year)
            .ToList();

        // Group by staff and calculate hours based on shift duration
        var staffHours = yearAssignments
            .GroupBy(a => a.StaffId)
            .Select(g => new
            {
                StaffId = g.Key,
                TotalHours = g.Sum(a => (a.Shift.EndTime - a.Shift.StartTime).TotalHours),
                TotalShifts = g.Count()
            })
            .ToList();

        // Get all staff to include their details
        var allStaff = await _staffRepository.GetAllStaffAsync();
        
        // Create DTOs with staff information
        var result = staffHours
            .Select(sh =>
            {
                var staff = allStaff.FirstOrDefault(s => s.Id == sh.StaffId);
                return new StaffHoursDTO
                {
                    StaffId = sh.StaffId,
                    StaffName = staff?.FullName ?? "Unknown",
                    Email = staff?.Email ?? "",
                    Year = query.Year,
                    TotalHours = Math.Round(sh.TotalHours, 2),
                    TotalShifts = sh.TotalShifts
                };
            })
            .OrderByDescending(sh => sh.TotalHours)
            .ToList();

        return result;
    }
}
