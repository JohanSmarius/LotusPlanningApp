using Application.Common;
using Entities;

namespace Application.Queries.Staff;

/// <summary>
/// Query to get active staff members
/// </summary>
public record GetActiveStaffQuery : IQuery<List<Entities.Staff>>;
