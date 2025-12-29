# ICS Calendar Download Feature

## Overview
This feature allows staff members to download ICS (iCalendar) files for shifts, making it easy to add shifts to personal calendar applications like Google Calendar, Outlook, Apple Calendar, etc.

## User Guide

### For Staff Members
1. Navigate to a shift's detail page
2. Click one of the "Download Calendar" or "Add to Calendar" buttons
3. Your browser will download a `.ics` file
4. Open the downloaded file or import it into your calendar application
5. The shift will be added to your calendar with all details

### What's Included in the Calendar Event
- **Event title**: Shift name + Event name (e.g., "Morning Shift - Christmas Medical Support")
- **Date & Time**: Start and end times (automatically converted to your timezone)
- **Location**: Event location
- **Description**: 
  - Shift name
  - Event name
  - Shift details/description
  - Required staff count
  - Assigned staff count
  - Current status
  - Location

## Technical Implementation

### Architecture
- **Interface**: `ICalendarService` in Application layer
- **Implementation**: `CalendarService` in Infrastructure layer
- **API Endpoint**: `CalendarController` with route `/api/calendar/shift/{shiftId}.ics`
- **UI**: Download buttons on `ShiftDetails.razor` page

### RFC 5545 Compliance
The implementation follows RFC 5545 (iCalendar) standard:
- Proper VCALENDAR and VEVENT structure
- UTC datetime format (`yyyyMMdd'T'HHmmss'Z'`)
- Text escaping for special characters (backslash, comma, semicolon, newlines)
- Unique UIDs per shift (`shift-{id}@lotus-planning-app`)
- Status mapping:
  - Open shifts: `TENTATIVE`
  - Full/InProgress/Completed shifts: `CONFIRMED`
  - Cancelled shifts: `CANCELLED`
- Priority: Medium priority (5) for understaffed shifts

### Security
- Authentication required: Only logged-in users can download calendar files
- Authorization: Admin and Lotus role members can view shifts and download calendars
- Filename sanitization: Prevents path traversal attacks
- Input validation: Shift ID validation, safe filename generation
- No SQL injection risks: Uses parameterized queries through CQRS handlers

### Testing
A console test application validates:
- All required RFC 5545 fields are present
- Proper structure (BEGIN/END blocks)
- Correct datetime formatting
- Text escaping
- Status and priority fields

## Files Changed
1. **Application/ICalendarService.cs** - Service interface
2. **Infrastructure/CalendarService.cs** - Service implementation
3. **LotusPlanningApp/LotusPlanningApp/Controllers/CalendarController.cs** - API controller
4. **LotusPlanningApp/LotusPlanningApp/Components/Pages/ShiftDetails.razor** - UI with download buttons
5. **LotusPlanningApp/LotusPlanningApp/Program.cs** - Service registration and controller mapping

## Future Enhancements
Possible improvements for the future:
- Add calendar download for entire events (all shifts)
- Email calendar invites automatically when staff is assigned
- Support for recurring shifts
- Add alarms/reminders to calendar events
- Download calendar for multiple selected shifts
- Add calendar feed subscription (live updating)
