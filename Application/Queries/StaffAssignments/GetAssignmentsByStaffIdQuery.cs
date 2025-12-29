using Application.Common;
using Entities;

namespace Application.Queries.StaffAssignments;

/// <summary>
/// Query to get assignments by staff ID
/// </summary>
public record GetAssignmentsByStaffIdQuery(int StaffId) : IQuery<List<StaffAssignment>>;
