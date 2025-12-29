using Application.Common;
using Entities;

namespace Application.Queries.Shifts;

/// <summary>
/// Query to get all shifts
/// </summary>
public record GetAllShiftsQuery : IQuery<List<Shift>>;
