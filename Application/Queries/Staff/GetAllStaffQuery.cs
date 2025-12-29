using Application.Common;
using Entities;

namespace Application.Queries.Staff;

/// <summary>
/// Query to get all staff members
/// </summary>
public record GetAllStaffQuery : IQuery<List<Entities.Staff>>;
