using Application.Common;
using Entities;

namespace Application.Queries.StaffAssignments;

/// <summary>
/// Query to get assignments by shift ID
/// </summary>
public record GetAssignmentsByShiftIdQuery(int ShiftId) : IQuery<List<StaffAssignment>>;
