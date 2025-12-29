using Application.Common;
using Entities;

namespace Application.Queries.Shifts;

/// <summary>
/// Query to get upcoming shifts
/// </summary>
public record GetUpcomingShiftsQuery : IQuery<List<Shift>>;
