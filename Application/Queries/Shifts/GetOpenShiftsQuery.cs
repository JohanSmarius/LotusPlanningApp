using Application.Common;
using Entities;

namespace Application.Queries.Shifts;

/// <summary>
/// Query to get all upcoming shifts with status Open
/// </summary>
public record GetOpenShiftsQuery : IQuery<List<Shift>>;
