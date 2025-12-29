using Application;
using Application.Queries.Shifts;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LotusPlanningApp.Controllers;

/// <summary>
/// Controller for calendar-related operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CalendarController : ControllerBase
{
    private readonly GetShiftByIdQueryHandler _getShiftByIdHandler;
    private readonly ICalendarService _calendarService;

    public CalendarController(
        GetShiftByIdQueryHandler getShiftByIdHandler,
        ICalendarService calendarService)
    {
        _getShiftByIdHandler = getShiftByIdHandler;
        _calendarService = calendarService;
    }

    /// <summary>
    /// Downloads an ICS calendar file for a specific shift
    /// </summary>
    /// <param name="shiftId">The ID of the shift</param>
    /// <returns>An ICS file for the shift</returns>
    [HttpGet("shift/{shiftId}.ics")]
    [Authorize] // Allow any authenticated user to download calendar for shifts
    public async Task<IActionResult> DownloadShiftCalendar(int shiftId)
    {
        try
        {
            // Get the shift details
            var query = new GetShiftByIdQuery(shiftId);
            var shift = await _getShiftByIdHandler.Handle(query);

            if (shift == null)
            {
                return NotFound(new { message = "Shift not found" });
            }

            // Generate the ICS file content
            var icsContent = _calendarService.GenerateShiftIcsFile(shift);

            // Create a safe filename
            var fileName = $"shift-{shift.Id}-{SanitizeFileName(shift.Name)}.ics";

            // Return the file
            return File(
                System.Text.Encoding.UTF8.GetBytes(icsContent),
                "text/calendar",
                fileName
            );
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while generating the calendar file", error = ex.Message });
        }
    }

    /// <summary>
    /// Sanitizes a filename by removing invalid characters
    /// </summary>
    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
        return sanitized.Length > 50 ? sanitized.Substring(0, 50) : sanitized;
    }
}
