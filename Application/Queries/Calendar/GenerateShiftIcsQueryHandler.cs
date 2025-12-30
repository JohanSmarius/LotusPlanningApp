using Application.Common;
using Entities;

namespace Application.Queries.Calendar;

/// <summary>
/// Handler for generating ICS calendar files for shifts
/// </summary>
public class GenerateShiftIcsQueryHandler : IQueryHandler<GenerateShiftIcsQuery, string?>
{
    private readonly IShiftRepository _shiftRepository;
    private readonly ICalendarService _calendarService;

    public GenerateShiftIcsQueryHandler(
        IShiftRepository shiftRepository,
        ICalendarService calendarService)
    {
        _shiftRepository = shiftRepository;
        _calendarService = calendarService;
    }

    public async Task<string?> Handle(GenerateShiftIcsQuery query, CancellationToken cancellationToken = default)
    {
        // Get the shift with all related data
        var shift = await _shiftRepository.GetShiftByIdAsync(query.ShiftId);
        
        if (shift == null)
        {
            return null;
        }

        // Generate the ICS file content
        return _calendarService.GenerateShiftIcsFile(shift);
    }
}
