using Infrastructure;
using LotusPlanningApp.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Infrastructure.Tests;

public class EmailServiceLocalizationTests
{
    [Theory]
    [InlineData("GenerateAssignmentEmailBody", "Diensttoewijzing")]
    [InlineData("GenerateAssignmentDeletionEmailBody", "Diensttoewijzing geannuleerd")]
    [InlineData("GenerateEventConfirmationEmailBody", "Evenementbevestiging")]
    [InlineData("GenerateEventPlannedEmailBody", "Update evenementplanning")]
    [InlineData("GenerateEventInvoiceEmailBody", "Factuur evenement")]
    public void GeneratedEmailBody_UsesDutchTitle(string methodName, string expectedTitle)
    {
        var service = new EmailService(
            Options.Create(new EmailOptions()),
            new ConfigurationBuilder().Build(),
            Mock.Of<ILogger<EmailService>>());

        var @event = new Entities.Event
        {
            Id = 1,
            Name = "Testevenement",
            Location = "Testlocatie",
            ContactPerson = "Testcontact"
        };
        var body = methodName switch
        {
            "GenerateAssignmentEmailBody" => service.GenerateAssignmentEmailBody(
                new Entities.Staff { FirstName = "Test", LastName = "Medewerker" },
                new Entities.Shift { Name = "Testdienst", EndTime = DateTime.UtcNow.AddHours(1) },
                @event),
            "GenerateAssignmentDeletionEmailBody" => service.GenerateAssignmentDeletionEmailBody(
                new Entities.Staff { FirstName = "Test", LastName = "Medewerker" },
                new Entities.Shift { Name = "Testdienst", EndTime = DateTime.UtcNow.AddHours(1) },
                @event),
            "GenerateEventConfirmationEmailBody" => service.GenerateEventConfirmationEmailBody(@event),
            "GenerateEventPlannedEmailBody" => service.GenerateEventPlannedEmailBody(@event),
            "GenerateEventInvoiceEmailBody" => service.GenerateEventInvoiceEmailBody(@event),
            _ => throw new ArgumentOutOfRangeException(nameof(methodName), methodName, null)
        };

        Assert.Contains($"<title>{expectedTitle}</title>", body);
    }
}
