using Application.Common;
using Entities;

namespace Application.Queries.Staff;

/// <summary>
/// Query to get a staff member by ID
/// </summary>
public record GetStaffByIdQuery(int StaffId) : IQuery<Entities.Staff?>;
