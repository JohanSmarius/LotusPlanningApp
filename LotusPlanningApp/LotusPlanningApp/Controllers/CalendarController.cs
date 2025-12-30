using Application.Queries.Calendar;
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
    private readonly GenerateShiftIcsQueryHandler _generateShiftIcsHandler;

    public CalendarController(GenerateShiftIcsQueryHandler generateShiftIcsHandler)
    {
        _generateShiftIcsHandler = generateShiftIcsHandler;
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
            // Execute the query to generate ICS content
            var query = new GenerateShiftIcsQuery(shiftId);
            var icsContent = await _generateShiftIcsHandler.Handle(query);

            if (icsContent == null)
            {
                return NotFound(new { message = "Shift not found" });
            }

            // Create a safe filename
            var fileName = $"shift-{shiftId}.ics";

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
}
