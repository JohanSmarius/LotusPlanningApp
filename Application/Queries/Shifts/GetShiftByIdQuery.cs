using Application.Common;
using Entities;

namespace Application.Queries.Shifts;

/// <summary>
/// Query to get a shift by ID
/// </summary>
public record GetShiftByIdQuery(int ShiftId) : IQuery<Shift?>;
