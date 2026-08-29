using LotusPlanningApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace LotusPlanningApp.Components.Account
{
    // Remove the "else if (EmailSender is IdentityNoOpEmailSender)" block from RegisterConfirmation.razor after updating with a real implementation.
    internal sealed class IdentityNoOpEmailSender : IEmailSender<ApplicationUser>
    {
        private readonly IEmailSender emailSender = new NoOpEmailSender();

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
            emailSender.SendEmailAsync(email, "Bevestig je e-mailadres", $"Bevestig je account door <a href='{confirmationLink}'>hier te klikken</a>.");

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
            emailSender.SendEmailAsync(email, "Stel je wachtwoord opnieuw in", $"Stel je wachtwoord opnieuw in door <a href='{resetLink}'>hier te klikken</a>.");

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
            emailSender.SendEmailAsync(email, "Stel je wachtwoord opnieuw in", $"Stel je wachtwoord opnieuw in met de volgende code: {resetCode}");
    }
}
