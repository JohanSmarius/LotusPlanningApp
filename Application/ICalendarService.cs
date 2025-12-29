using Entities;

namespace Application;

/// <summary>
/// Service for generating iCalendar (.ics) files
/// </summary>
public interface ICalendarService
{
    /// <summary>
    /// Generates an iCalendar (.ics) file content for a shift
    /// </summary>
    /// <param name="shift">The shift to create a calendar event for</param>
    /// <returns>The iCalendar file content as a string</returns>
    string GenerateShiftIcsFile(Shift shift);
}
