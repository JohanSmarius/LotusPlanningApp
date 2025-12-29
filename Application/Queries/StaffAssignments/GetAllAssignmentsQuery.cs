using Application.Common;
using Entities;

namespace Application.Queries.StaffAssignments;

/// <summary>
/// Query to get all staff assignments
/// </summary>
public record GetAllAssignmentsQuery : IQuery<List<StaffAssignment>>;
