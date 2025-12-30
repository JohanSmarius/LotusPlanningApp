using Application.Common;
using Entities;

namespace Application.Queries.StaffAssignments;

/// <summary>
/// Query to get assignments by staff ID where the event status is Confirmed
/// </summary>
public record GetConfirmedAssignmentsByStaffIdQuery(int StaffId, DateTime CutoffDate) : IQuery<List<StaffAssignment>>;
