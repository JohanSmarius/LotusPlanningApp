using Application.Common;
using Application.DataAdapters;

namespace Application.Queries.StaffAssignments;

/// <summary>
/// Query to get aggregated hours worked per year for all staff members
/// </summary>
public record GetStaffHoursPerYearQuery(int Year) : IQuery<List<StaffHoursDTO>>;
