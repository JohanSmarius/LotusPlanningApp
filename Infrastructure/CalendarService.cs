using System.Text;
using Entities;
using Application;

namespace Infrastructure;

/// <summary>
/// Service for generating iCalendar (.ics) files according to RFC 5545
/// </summary>
public class CalendarService : ICalendarService
{
    // Priority constants according to RFC 5545 (1=highest, 9=lowest, 0=undefined)
    private const int PRIORITY_MEDIUM = 5;
    
    /// <summary>
    /// Generates an iCalendar (.ics) file content for a shift
    /// </summary>
    /// <param name="shift">The shift to create a calendar event for</param>
    /// <returns>The iCalendar file content as a string</returns>
    public string GenerateShiftIcsFile(Shift shift)
    {
        var icsBuilder = new StringBuilder();
        
        // iCalendar header
        icsBuilder.AppendLine("BEGIN:VCALENDAR");
        icsBuilder.AppendLine("VERSION:2.0");
        icsBuilder.AppendLine("PRODID:-//LOTUS Planning App//Shift Calendar//EN");
        icsBuilder.AppendLine("CALSCALE:GREGORIAN");
        icsBuilder.AppendLine("METHOD:PUBLISH");
        
        // Event (VEVENT)
        icsBuilder.AppendLine("BEGIN:VEVENT");
        
        // Unique identifier for the event
        icsBuilder.AppendLine($"UID:shift-{shift.Id}@lotus-planning-app");
        
        // Date/time stamp (when the iCalendar file was created)
        var now = DateTime.UtcNow;
        icsBuilder.AppendLine($"DTSTAMP:{FormatDateTime(now)}");
        
        // Start and end times (convert to UTC)
        // Handle DateTime with Unspecified kind by treating it as local time
        var startUtc = shift.StartTime.Kind == DateTimeKind.Unspecified 
            ? DateTime.SpecifyKind(shift.StartTime, DateTimeKind.Local).ToUniversalTime()
            : shift.StartTime.ToUniversalTime();
        var endUtc = shift.EndTime.Kind == DateTimeKind.Unspecified 
            ? DateTime.SpecifyKind(shift.EndTime, DateTimeKind.Local).ToUniversalTime()
            : shift.EndTime.ToUniversalTime();
        icsBuilder.AppendLine($"DTSTART:{FormatDateTime(startUtc)}");
        icsBuilder.AppendLine($"DTEND:{FormatDateTime(endUtc)}");
        
        // Event title (summary)
        var summary = EscapeText($"{shift.Name} - {shift.Event.Name}");
        icsBuilder.AppendLine($"SUMMARY:{summary}");
        
        // Event description
        var description = BuildDescription(shift);
        icsBuilder.AppendLine($"DESCRIPTION:{EscapeText(description)}");
        
        // Location
        if (!string.IsNullOrEmpty(shift.Event.Location))
        {
            icsBuilder.AppendLine($"LOCATION:{EscapeText(shift.Event.Location)}");
        }
        
        // Status according to RFC 5545
        // TENTATIVE: Event is not confirmed yet
        // CONFIRMED: Event is confirmed
        // CANCELLED: Event has been cancelled
        var status = shift.Status switch
        {
            ShiftStatus.Cancelled => "CANCELLED",
            ShiftStatus.Completed => "CONFIRMED",
            ShiftStatus.InProgress => "CONFIRMED",
            ShiftStatus.Full => "CONFIRMED",
            ShiftStatus.Open => "TENTATIVE", // Open shifts are not yet confirmed/finalized
            _ => "TENTATIVE"
        };
        icsBuilder.AppendLine($"STATUS:{status}");
        
        // Priority (optional, higher for urgent shifts)
        if (shift.StaffAssignments.Count < shift.RequiredStaff)
        {
            icsBuilder.AppendLine($"PRIORITY:{PRIORITY_MEDIUM}"); // Medium priority for understaffed shifts
        }
        
        icsBuilder.AppendLine("END:VEVENT");
        icsBuilder.AppendLine("END:VCALENDAR");
        
        return icsBuilder.ToString();
    }
    
    /// <summary>
    /// Formats a DateTime to iCalendar format (yyyyMMddTHHmmssZ)
    /// </summary>
    private static string FormatDateTime(DateTime dateTime)
    {
        return dateTime.ToString("yyyyMMdd'T'HHmmss'Z'");
    }
    
    /// <summary>
    /// Escapes text according to RFC 5545 (backslash escaping for special characters)
    /// </summary>
    private static string EscapeText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;
        
        return text
            .Replace("\\", "\\\\")
            .Replace(",", "\\,")
            .Replace(";", "\\;")
            .Replace("\n", "\\n")
            .Replace("\r", "");
    }
    
    /// <summary>
    /// Builds a detailed description for the shift
    /// </summary>
    private static string BuildDescription(Shift shift)
    {
        var descBuilder = new StringBuilder();
        
        descBuilder.AppendLine($"Shift: {shift.Name}");
        descBuilder.AppendLine($"Event: {shift.Event.Name}");
        
        if (!string.IsNullOrEmpty(shift.Description))
        {
            descBuilder.AppendLine($"Details: {shift.Description}");
        }
        
        descBuilder.AppendLine($"Required Staff: {shift.RequiredStaff}");
        descBuilder.AppendLine($"Assigned Staff: {shift.StaffAssignments.Count}");
        descBuilder.AppendLine($"Status: {shift.Status}");
        
        if (!string.IsNullOrEmpty(shift.Event.Location))
        {
            descBuilder.AppendLine($"Location: {shift.Event.Location}");
        }
        
        return descBuilder.ToString();
    }
}
