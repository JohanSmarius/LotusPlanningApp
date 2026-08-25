using Application;
using Application.Commands.Events;
using Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LotusPlanningApp.Controllers;

/// <summary>
/// Controller for invoice-related operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InvoiceController : ControllerBase
{
    private readonly GenerateInvoicePdfCommandHandler _generateInvoicePdfHandler;
    private readonly IPdfService _pdfService;
    private readonly IEventRepository _eventRepository;

    /// <summary>
    /// Initializes a new instance of <see cref="InvoiceController"/>
    /// </summary>
    public InvoiceController(
        GenerateInvoicePdfCommandHandler generateInvoicePdfHandler,
        IPdfService pdfService,
        IEventRepository eventRepository)
    {
        _generateInvoicePdfHandler = generateInvoicePdfHandler;
        _pdfService = pdfService;
        _eventRepository = eventRepository;
    }

    /// <summary>
    /// Generates and downloads a PDF invoice for a specific event
    /// </summary>
    /// <param name="eventId">The ID of the event to invoice</param>
    /// <param name="poNumber">The customer's Purchase Order number</param>
    /// <param name="sendEmail">Whether to also send the invoice by email (default: true)</param>
    /// <returns>A PDF file for the invoice</returns>
    [HttpGet("event/{eventId}/pdf")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DownloadInvoicePdf(
        int eventId,
        [FromQuery] string poNumber,
        [FromQuery] bool sendEmail = true)
    {
        if (string.IsNullOrWhiteSpace(poNumber))
        {
            return BadRequest(new { message = "PO-nummer is verplicht." });
        }

        try
        {
            byte[] pdfBytes;

            if (sendEmail)
            {
                // Generate PDF and email it to the customer
                var command = new GenerateInvoicePdfCommand(eventId, poNumber);
                pdfBytes = await _generateInvoicePdfHandler.Handle(command);
            }
            else
            {
                // Only generate PDF without emailing
                var @event = await _eventRepository.GetEventByIdAsync(eventId);
                if (@event is null)
                {
                    return NotFound(new { message = $"Opdracht met ID {eventId} niet gevonden." });
                }
                pdfBytes = _pdfService.GenerateInvoicePdf(@event, poNumber);
            }

            var invoiceNumber = $"INV-{eventId:D6}-{DateTime.UtcNow:yyyyMM}";
            var fileName = $"Factuur-{invoiceNumber}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(new { message = $"Opdracht met ID {eventId} niet gevonden." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Er is een fout opgetreden bij het genereren van de factuur.", error = ex.Message });
        }
    }
}
