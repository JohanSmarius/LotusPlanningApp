using Entities;

namespace Application;

/// <summary>
/// Service for generating PDF documents
/// </summary>
public interface IPdfService
{
    /// <summary>
    /// Generates a PDF invoice for an event
    /// </summary>
    /// <param name="event">The event to invoice</param>
    /// <param name="purchaseOrderNumber">The customer PO number to include on the invoice</param>
    /// <returns>PDF content as a byte array</returns>
    byte[] GenerateInvoicePdf(Event @event, string purchaseOrderNumber);
}
