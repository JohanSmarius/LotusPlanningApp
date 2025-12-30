using Application.Common;

namespace Application.Queries.Calendar;

/// <summary>
/// Query to generate an ICS calendar file for a shift
/// </summary>
public record GenerateShiftIcsQuery(int ShiftId) : IQuery<string?>;
