using Application.Commands.Events;
using Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Application.Tests.Commands.Events;

public class GenerateInvoicePdfCommandHandlerTests
{
    private readonly Mock<IEventRepository> _mockEventRepository;
    private readonly Mock<IPdfService> _mockPdfService;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<ILogger<GenerateInvoicePdfCommandHandler>> _mockLogger;
    private readonly GenerateInvoicePdfCommandHandler _handler;

    public GenerateInvoicePdfCommandHandlerTests()
    {
        _mockEventRepository = new Mock<IEventRepository>();
        _mockPdfService = new Mock<IPdfService>();
        _mockEmailService = new Mock<IEmailService>();
        _mockLogger = new Mock<ILogger<GenerateInvoicePdfCommandHandler>>();

        _handler = new GenerateInvoicePdfCommandHandler(
            _mockEventRepository.Object,
            _mockPdfService.Object,
            _mockEmailService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_EventNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var command = new GenerateInvoicePdfCommand(99, "PO-001");
        _mockEventRepository.Setup(r => r.GetEventByIdAsync(99)).ReturnsAsync((Event?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command));
    }

    [Fact]
    public async Task Handle_EventFound_ReturnsPdfBytes()
    {
        // Arrange
        var @event = CreateTestEvent();
        var expectedPdf = new byte[] { 1, 2, 3 };
        var command = new GenerateInvoicePdfCommand(@event.Id, "PO-2024-001");

        _mockEventRepository.Setup(r => r.GetEventByIdAsync(@event.Id)).ReturnsAsync(@event);
        _mockPdfService.Setup(p => p.GenerateInvoicePdf(@event, "PO-2024-001")).Returns(expectedPdf);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.Equal(expectedPdf, result);
        _mockPdfService.Verify(p => p.GenerateInvoicePdf(@event, "PO-2024-001"), Times.Once);
    }

    [Fact]
    public async Task Handle_EventWithCustomerEmail_SendsInvoiceEmail()
    {
        // Arrange
        var @event = CreateTestEvent();
        @event.Customer = new Customer
        {
            Id = 1,
            FirstName = "Test",
            LastName = "Klant",
            Email = "klant@example.com"
        };

        var pdfBytes = new byte[] { 1, 2, 3 };
        var command = new GenerateInvoicePdfCommand(@event.Id, "PO-2024-001");

        _mockEventRepository.Setup(r => r.GetEventByIdAsync(@event.Id)).ReturnsAsync(@event);
        _mockPdfService.Setup(p => p.GenerateInvoicePdf(@event, "PO-2024-001")).Returns(pdfBytes);

        // Act
        await _handler.Handle(command);

        // Assert
        _mockEmailService.Verify(
            e => e.SendInvoicePdfEmailAsync(@event, pdfBytes, "PO-2024-001"),
            Times.Once);
    }

    [Fact]
    public async Task Handle_EventWithContactEmailButNoCustomer_SendsInvoiceEmail()
    {
        // Arrange
        var @event = CreateTestEvent();
        @event.ContactEmail = "contact@example.com";
        @event.Customer = null;

        var pdfBytes = new byte[] { 4, 5, 6 };
        var command = new GenerateInvoicePdfCommand(@event.Id, "PO-TEST");

        _mockEventRepository.Setup(r => r.GetEventByIdAsync(@event.Id)).ReturnsAsync(@event);
        _mockPdfService.Setup(p => p.GenerateInvoicePdf(@event, "PO-TEST")).Returns(pdfBytes);

        // Act
        await _handler.Handle(command);

        // Assert
        _mockEmailService.Verify(
            e => e.SendInvoicePdfEmailAsync(@event, pdfBytes, "PO-TEST"),
            Times.Once);
    }

    [Fact]
    public async Task Handle_EventWithNoEmail_DoesNotSendInvoiceEmail()
    {
        // Arrange
        var @event = CreateTestEvent();
        @event.Customer = null;
        @event.ContactEmail = null;

        var pdfBytes = new byte[] { 7, 8, 9 };
        var command = new GenerateInvoicePdfCommand(@event.Id, "PO-NO-EMAIL");

        _mockEventRepository.Setup(r => r.GetEventByIdAsync(@event.Id)).ReturnsAsync(@event);
        _mockPdfService.Setup(p => p.GenerateInvoicePdf(@event, "PO-NO-EMAIL")).Returns(pdfBytes);

        // Act
        await _handler.Handle(command);

        // Assert
        _mockEmailService.Verify(
            e => e.SendInvoicePdfEmailAsync(It.IsAny<Event>(), It.IsAny<byte[]>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_EmailServiceThrows_StillReturnsPdfBytes()
    {
        // Arrange
        var @event = CreateTestEvent();
        @event.ContactEmail = "contact@example.com";

        var pdfBytes = new byte[] { 1, 2, 3 };
        var command = new GenerateInvoicePdfCommand(@event.Id, "PO-ERR");

        _mockEventRepository.Setup(r => r.GetEventByIdAsync(@event.Id)).ReturnsAsync(@event);
        _mockPdfService.Setup(p => p.GenerateInvoicePdf(@event, "PO-ERR")).Returns(pdfBytes);
        _mockEmailService
            .Setup(e => e.SendInvoicePdfEmailAsync(It.IsAny<Event>(), It.IsAny<byte[]>(), It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("Email service unavailable"));

        // Act
        var result = await _handler.Handle(command);

        // Assert - PDF bytes returned even when email fails
        Assert.Equal(pdfBytes, result);
    }

    private static Event CreateTestEvent() => new Event
    {
        Id = 1,
        Name = "Test Evenement",
        Location = "Tilburg",
        StartDate = new DateTime(2025, 6, 1, 10, 0, 0),
        EndDate = new DateTime(2025, 6, 1, 18, 0, 0),
        Status = EventStatus.SendInvoice
    };
}
