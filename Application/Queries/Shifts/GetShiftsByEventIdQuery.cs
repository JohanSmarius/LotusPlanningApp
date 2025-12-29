using Application.Common;
using Entities;

namespace Application.Queries.Shifts;

/// <summary>
/// Query to get shifts by event ID
/// </summary>
public record GetShiftsByEventIdQuery(int EventId) : IQuery<List<Shift>>;
