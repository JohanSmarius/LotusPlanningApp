using Application.Common;

namespace Application.Queries.StaffAssignments;

/// <summary>
/// Query to check if a staff member is available during a specific time period
/// </summary>
public record IsStaffAvailableQuery(int StaffId, DateTime StartTime, DateTime EndTime) : IQuery<bool>;
