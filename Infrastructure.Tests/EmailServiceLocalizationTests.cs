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
            AssertDutchTitle(service => service.GenerateAssignmentEmailBody(CreateStaff(), CreateShift(), CreateEvent()), "Diensttoewijzing");
        }

        [Fact]
        public void AssignmentDeletionEmailBody_UsesDutchTitle()
        {
            AssertDutchTitle(service => service.GenerateAssignmentDeletionEmailBody(CreateStaff(), CreateShift(), CreateEvent()), "Diensttoewijzing geannuleerd");
        }

        [Fact]
        public void EventConfirmationEmailBody_UsesDutchTitle()
        {
            AssertDutchTitle(service => service.GenerateEventConfirmationEmailBody(CreateEvent()), "Evenementbevestiging");
        }

        [Fact]
        public void EventPlannedEmailBody_UsesDutchTitle()
        {
            AssertDutchTitle(service => service.GenerateEventPlannedEmailBody(CreateEvent()), "Update evenementplanning");
        }

        [Fact]
        public void EventInvoiceEmailBody_UsesDutchTitle()
        {
            AssertDutchTitle(service => service.GenerateEventInvoiceEmailBody(CreateEvent()), "Factuur evenement");
        }

        private static void AssertDutchTitle(Func<EmailService, string> generateBody, string expectedTitle)
        {
            var service = new EmailService(
                Options.Create(new EmailOptions()),
                new ConfigurationBuilder().Build(),
                Mock.Of<ILogger<EmailService>>());

            Assert.Contains($"<title>{expectedTitle}</title>", generateBody(service));
        }

        private static Entities.Staff CreateStaff() => new() { FirstName = "Test", LastName = "Medewerker" };

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
            ContactPerson = "Testcontact"
        };
    }
