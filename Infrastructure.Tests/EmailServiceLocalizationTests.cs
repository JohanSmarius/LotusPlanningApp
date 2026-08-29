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
        [Fact]
        public void AssignmentEmailBody_UsesDutchTitle()
        {
            AssertDutchContent(service => service.GenerateAssignmentEmailBody(CreateStaff(), CreateShift(), CreateEvent()), "Diensttoewijzing", "Hallo");
        }

        [Fact]
        public void AssignmentDeletionEmailBody_UsesDutchTitle()
        {
            AssertDutchContent(service => service.GenerateAssignmentDeletionEmailBody(CreateStaff(), CreateShift(), CreateEvent()), "Diensttoewijzing geannuleerd", "Hallo");
        }

        [Fact]
        public void EventConfirmationEmailBody_UsesDutchTitle()
        {
            AssertDutchContent(service => service.GenerateEventConfirmationEmailBody(CreateEvent()), "Evenementbevestiging", "Hallo");
        }

        [Fact]
        public void EventPlannedEmailBody_UsesDutchTitle()
        {
            AssertDutchContent(service => service.GenerateEventPlannedEmailBody(CreateEvent()), "Update evenementplanning", "Hallo");
        }

        [Fact]
        public void EventInvoiceEmailBody_UsesDutchTitle()
        {
            AssertDutchContent(service => service.GenerateEventInvoiceEmailBody(CreateEvent()), "Factuur evenement", "Beste");
        }

        private static void AssertDutchContent(Func<EmailService, string> generateBody, string expectedTitle, string expectedGreeting)
        {
            var service = new EmailService(
                Options.Create(new EmailOptions()),
                new ConfigurationBuilder().Build(),
                Mock.Of<ILogger<EmailService>>());

            var body = generateBody(service);
            Assert.Contains($"<title>{expectedTitle}</title>", body);
            Assert.Contains($"<h1>", body);
            Assert.Contains(expectedTitle, body);
            Assert.Contains(expectedGreeting, body);
        }

        private static Entities.Staff CreateStaff() => new()
        {
            FirstName = "Test",
            LastName = "Medewerker",
            Email = "test@example.com"
        };

        private static Entities.Shift CreateShift()
        {
            var startTime = DateTime.UtcNow;
            return new Entities.Shift { Name = "Testdienst", StartTime = startTime, EndTime = startTime.AddHours(1) };
        }

        private static Entities.Event CreateEvent() => new()
        {
            Id = 1,
            Name = "Testevenement",
            Location = "Testlocatie",
            ContactPerson = "Testcontact",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddHours(2)
        };
    }
