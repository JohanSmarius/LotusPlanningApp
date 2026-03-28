using Application.Common;
using Entities;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Events;

/// <summary>
/// Handler for generating a PDF invoice for an event and emailing it to the customer
/// </summary>
public class GenerateInvoicePdfCommandHandler : ICommandHandler<GenerateInvoicePdfCommand, byte[]>
{
    private readonly IEventRepository _eventRepository;
    private readonly IPdfService _pdfService;
    private readonly IEmailService _emailService;
    private readonly ILogger<GenerateInvoicePdfCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="GenerateInvoicePdfCommandHandler"/>
    /// </summary>
    public GenerateInvoicePdfCommandHandler(
        IEventRepository eventRepository,
        IPdfService pdfService,
        IEmailService emailService,
        ILogger<GenerateInvoicePdfCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _pdfService = pdfService;
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>
    /// Generates a PDF invoice and emails it to the customer
    /// </summary>
    public async Task<byte[]> Handle(GenerateInvoicePdfCommand command, CancellationToken cancellationToken = default)
    {
        var @event = await _eventRepository.GetEventByIdAsync(command.EventId)
            ?? throw new InvalidOperationException($"Event {command.EventId} not found");

        var pdfBytes = _pdfService.GenerateInvoicePdf(@event, command.PurchaseOrderNumber);

        // Send the invoice PDF by email if a recipient address is available
        var recipientEmail = @event.Customer?.Email ?? @event.ContactEmail;
        if (!string.IsNullOrEmpty(recipientEmail))
        {
            try
            {
                await _emailService.SendInvoicePdfEmailAsync(@event, pdfBytes, command.PurchaseOrderNumber);
                _logger.LogInformation(
                    "Invoice PDF emailed to {Email} for event {EventId}", recipientEmail, @event.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send invoice PDF for event {EventId}", @event.Id);
            }
        }
        else
        {
            _logger.LogWarning(
                "No recipient email found for event {EventId}; invoice PDF not mailed", @event.Id);
        }

        return pdfBytes;
    }
}
