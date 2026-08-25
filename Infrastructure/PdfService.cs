using Application;
using Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure;

/// <summary>
/// PDF generation service that produces invoice documents using QuestPDF
/// </summary>
public class PdfService : IPdfService
{
    /// <summary>
    /// Generates a PDF invoice for an event
    /// </summary>
    public byte[] GenerateInvoicePdf(Event @event, string purchaseOrderNumber)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var invoiceNumber = $"INV-{@event.Id:D6}-{DateTime.UtcNow:yyyyMM}";
        var invoiceDate = DateTime.UtcNow;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                page.Header().Element(c => ComposeHeader(c, invoiceNumber, invoiceDate));
                page.Content().Element(c => ComposeContent(c, @event, purchaseOrderNumber, invoiceNumber, invoiceDate));
                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    private static void ComposeHeader(IContainer container, string invoiceNumber, DateTime invoiceDate)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                // Company info (left)
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("LOTUS").FontSize(26).Bold().FontColor(Colors.Blue.Darken2);
                    c.Item().Text("Medische Eerste Hulp & Evenementenbeveiliging")
                        .FontSize(10).FontColor(Colors.Grey.Darken1);
                    c.Item().Text("info@lotus-tilburg.nl").FontSize(9).FontColor(Colors.Grey.Medium);
                });

                // Invoice label (right)
                row.ConstantItem(160).Column(c =>
                {
                    c.Item().AlignRight().Text("FACTUUR").FontSize(22).Bold().FontColor(Colors.Blue.Darken2);
                    c.Item().AlignRight().Text($"Nummer: {invoiceNumber}").FontSize(9);
                    c.Item().AlignRight().Text($"Datum: {invoiceDate:d MMMM yyyy}").FontSize(9);
                    c.Item().AlignRight().Text($"Betalingstermijn: 30 dagen").FontSize(9);
                });
            });

            col.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Blue.Darken2);
        });
    }

    private static void ComposeContent(
        IContainer container,
        Event @event,
        string purchaseOrderNumber,
        string invoiceNumber,
        DateTime invoiceDate)
    {
        container.Column(col =>
        {
            col.Spacing(12);

            // Billing addresses row
            col.Item().PaddingTop(8).Row(row =>
            {
                // Bill to
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("FACTUURADRES").FontSize(8).Bold().FontColor(Colors.Grey.Darken1);
                    c.Item().PaddingTop(4).Element(inner => ComposeBillToSection(inner, @event));
                });

                // Invoice details
                row.ConstantItem(200).Column(c =>
                {
                    c.Item().Text("FACTUURGEGEVENS").FontSize(8).Bold().FontColor(Colors.Grey.Darken1);
                    c.Item().PaddingTop(4).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(4);
                        });

                        table.Cell().Text("Factuurnummer:").Bold();
                        table.Cell().Text(invoiceNumber);
                        table.Cell().Text("Factuurdatum:").Bold();
                        table.Cell().Text(invoiceDate.ToString("d MMMM yyyy"));
                        table.Cell().Text("Vervaldatum:").Bold();
                        table.Cell().Text(invoiceDate.AddDays(30).ToString("d MMMM yyyy"));
                        table.Cell().Text("PO-nummer:").Bold();
                        table.Cell().Text(purchaseOrderNumber);
                    });
                });
            });

            // Event details card
            col.Item().Background(Colors.Blue.Lighten5).Padding(10).Column(c =>
            {
                c.Item().Text("OPDRACHTDETAILS").FontSize(8).Bold().FontColor(Colors.Blue.Darken2);
                c.Item().PaddingTop(4).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(120);
                        columns.RelativeColumn();
                    });

                    table.Cell().Text("Opdrachtnaam:").Bold();
                    table.Cell().Text(@event.Name);
                    table.Cell().Text("Locatie:").Bold();
                    table.Cell().Text(@event.Location);
                    table.Cell().Text("Startdatum:").Bold();
                    table.Cell().Text(@event.StartDate.ToString("dddd d MMMM yyyy HH:mm"));
                    table.Cell().Text("Einddatum:").Bold();
                    table.Cell().Text(@event.EndDate.ToString("dddd d MMMM yyyy HH:mm"));

                    if (!string.IsNullOrEmpty(@event.Description))
                    {
                        table.Cell().Text("Omschrijving:").Bold();
                        table.Cell().Text(@event.Description);
                    }
                });
            });

            // Line items table
            col.Item().Element(c => ComposeLineItems(c, @event));

            // Totals
            col.Item().Element(c => ComposeTotals(c, @event));

            // Payment info
            col.Item().PaddingTop(8).Background(Colors.Grey.Lighten4).Padding(10).Column(c =>
            {
                c.Item().Text("BETALINGSINFORMATIE").FontSize(8).Bold().FontColor(Colors.Grey.Darken2);
                c.Item().PaddingTop(4).Text(
                    "Gelieve het openstaande bedrag binnen 30 dagen na factuurdatum over te maken. " +
                    "Bij vragen over deze factuur kunt u contact opnemen via info@lotus-tilburg.nl.").FontSize(9);
            });
        });
    }

    private static void ComposeBillToSection(IContainer container, Event @event)
    {
        container.Column(col =>
        {
            if (@event.Customer is not null)
            {
                if (!string.IsNullOrEmpty(@event.Customer.Company))
                    col.Item().Text(@event.Customer.Company).Bold();

                col.Item().Text(@event.Customer.FullName);

                if (!string.IsNullOrEmpty(@event.Customer.Address))
                    col.Item().Text(@event.Customer.Address);

                var cityLine = string.Join(" ", new[]
                {
                    @event.Customer.PostalCode,
                    @event.Customer.City
                }.Where(s => !string.IsNullOrEmpty(s)));
                if (!string.IsNullOrEmpty(cityLine))
                    col.Item().Text(cityLine);

                if (!string.IsNullOrEmpty(@event.Customer.Country))
                    col.Item().Text(@event.Customer.Country);

                col.Item().PaddingTop(4).Text(@event.Customer.Email).FontColor(Colors.Grey.Darken1);
            }
            else
            {
                if (!string.IsNullOrEmpty(@event.ContactPerson))
                    col.Item().Text(@event.ContactPerson).Bold();
                if (!string.IsNullOrEmpty(@event.ContactEmail))
                    col.Item().Text(@event.ContactEmail).FontColor(Colors.Grey.Darken1);
                if (!string.IsNullOrEmpty(@event.ContactPhone))
                    col.Item().Text(@event.ContactPhone).FontColor(Colors.Grey.Darken1);
            }
        });
    }

    private static void ComposeLineItems(IContainer container, Event @event)
    {
        container.Table(table =>
        {
            // Column definitions
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(4);   // Description
                columns.RelativeColumn(2);   // Date
                columns.RelativeColumn(1.5f); // Duration
                columns.RelativeColumn(1.5f); // Staff
                columns.RelativeColumn(2);   // Hours
            });

            // Header row
            static IContainer HeaderCell(IContainer c) =>
                c.Background(Colors.Blue.Darken2).Padding(6).AlignMiddle();

            table.Header(header =>
            {
                header.Cell().Element(HeaderCell).Text("Omschrijving").FontColor(Colors.White).Bold().FontSize(9);
                header.Cell().Element(HeaderCell).Text("Datum").FontColor(Colors.White).Bold().FontSize(9);
                header.Cell().Element(HeaderCell).AlignRight().Text("Duur").FontColor(Colors.White).Bold().FontSize(9);
                header.Cell().Element(HeaderCell).AlignRight().Text("LOTUS").FontColor(Colors.White).Bold().FontSize(9);
                header.Cell().Element(HeaderCell).AlignRight().Text("Totaal uren").FontColor(Colors.White).Bold().FontSize(9);
            });

            // Data rows
            var isOdd = true;
            foreach (var shift in @event.Shifts.OrderBy(s => s.StartTime))
            {
                var bgColor = isOdd ? Colors.White : Colors.Grey.Lighten5;
                isOdd = !isOdd;

                static IContainer DataCell(IContainer c, string color) =>
                    c.Background(color).Padding(6).AlignMiddle();

                var duration = shift.EndTime - shift.StartTime;
                var durationHours = duration.TotalHours;
                var durationText = duration.TotalHours >= 24
                    ? $"{duration.Days}d {duration.Hours}u {duration.Minutes}m"
                    : $"{duration.Hours}u {duration.Minutes:D2}m";

                var staffCount = shift.StaffAssignments?.Count > 0
                    ? shift.StaffAssignments.Count
                    : shift.RequiredStaff;
                var totalHours = durationHours * staffCount;

                table.Cell().Element(c => DataCell(c, bgColor)).Column(inner =>
                {
                    inner.Item().Text(shift.Name).Bold().FontSize(9);
                    if (!string.IsNullOrEmpty(shift.Description))
                        inner.Item().Text(shift.Description).FontSize(8).FontColor(Colors.Grey.Darken1);
                });
                table.Cell().Element(c => DataCell(c, bgColor))
                    .Text(shift.StartTime.ToString("d MMM yyyy")).FontSize(9);
                table.Cell().Element(c => DataCell(c, bgColor))
                    .AlignRight().Text(durationText).FontSize(9);
                table.Cell().Element(c => DataCell(c, bgColor))
                    .AlignRight().Text(staffCount.ToString()).FontSize(9);
                table.Cell().Element(c => DataCell(c, bgColor))
                    .AlignRight().Text($"{totalHours:F2}u").FontSize(9);
            }

            // If no shifts, show a placeholder row
            if (!@event.Shifts.Any())
            {
                table.Cell().ColumnSpan(5).Padding(10)
                    .Text("Geen deelopdrachten gevonden voor deze opdracht.")
                    .FontColor(Colors.Grey.Medium).Italic();
            }
        });
    }

    private static void ComposeTotals(IContainer container, Event @event)
    {
        var totalHours = @event.Shifts.Sum(shift =>
        {
            var duration = (shift.EndTime - shift.StartTime).TotalHours;
            var staffCount = shift.StaffAssignments?.Count > 0
                ? shift.StaffAssignments.Count
                : shift.RequiredStaff;
            return duration * staffCount;
        });

        container.AlignRight().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(3);
                columns.RelativeColumn(2);
            });

            table.Cell().PaddingVertical(4).Text("Totaal uren:").Bold();
            table.Cell().PaddingVertical(4).AlignRight().Text($"{totalHours:F2} uur").Bold();

            table.Cell().ColumnSpan(2)
                .PaddingTop(2).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);

            table.Cell().PaddingVertical(4)
                .Text("Bedrag").Bold().FontSize(9).FontColor(Colors.Grey.Darken1);
            table.Cell().PaddingVertical(4).AlignRight()
                .Text("Zie bijgevoegde prijsopgave").FontSize(9).FontColor(Colors.Grey.Darken1).Italic();
        });
    }

    private static void ComposeFooter(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);
            col.Item().PaddingTop(4).Row(row =>
            {
                row.RelativeItem().Text("LOTUS - Medische Eerste Hulp").FontSize(8).FontColor(Colors.Grey.Medium);
                row.RelativeItem().AlignCenter()
                    .Text("info@lotus-tilburg.nl").FontSize(8).FontColor(Colors.Grey.Medium);
                row.RelativeItem().AlignRight()
                    .Text(text =>
                    {
                        text.Span("Pagina ").FontSize(8).FontColor(Colors.Grey.Medium);
                        text.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                        text.Span(" van ").FontSize(8).FontColor(Colors.Grey.Medium);
                        text.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                    });
            });
        });
    }
}
