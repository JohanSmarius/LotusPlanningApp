using Application.Common;

namespace Application.Commands.Events;

/// <summary>
/// Command for generating a PDF invoice for an event and sending it to the customer
/// </summary>
/// <param name="EventId">The ID of the event to invoice</param>
/// <param name="PurchaseOrderNumber">The customer's PO number to include on the invoice</param>
public record GenerateInvoicePdfCommand(int EventId, string PurchaseOrderNumber) : ICommand<byte[]>;
