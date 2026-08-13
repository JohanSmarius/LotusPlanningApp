using Application.Common;
using Entities;

namespace Application.Queries.Shifts;

/// <summary>
/// Query to get shifts with an end date in the past.
/// </summary>
/// <param name="ReferenceDate">Optional reference date used to determine if a shift has ended.</param>
public record GetShiftsPastEndDateQuery(DateTime? ReferenceDate = null) : IQuery<List<Shift>>;
