using System.Reflection;
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

        var method = typeof(EmailService).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(method);
        var arguments = methodName.Contains("Assignment")
            ? new object[] { new Entities.Staff(), new Entities.Shift(), new Entities.Event() }
            : new object[] { new Entities.Event() };

        var body = (string)method!.Invoke(service, arguments)!;

        Assert.Contains($"<title>{expectedTitle}</title>", body);
    }
}
