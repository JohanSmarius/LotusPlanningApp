using LotusPlanningApp.Configuration;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;
using Entities;
using Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure;

/// <summary>
/// SMTP-based email service for sending notifications
/// </summary>
public class EmailService : IEmailService
{
    private readonly IOptions<EmailOptions> _emailOptions;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailOptions> emailOptions, IConfiguration configuration, ILogger<EmailService> logger)
    {
        _emailOptions = emailOptions;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Sends a staff assignment notification email
    /// </summary>
    public async Task SendStaffAssignmentNotificationAsync(Staff staff, Shift shift, Event @event)
    {
        var subject = $"Dienst toegewezen: {@event.Name}";
        var htmlBody = GenerateAssignmentEmailBody(staff, shift, @event);
        
        await SendEmailAsync(staff.Email, subject, htmlBody);
    }

    /// <summary>
    /// Sends a staff assignment deletion notification email
    /// </summary>
    public async Task SendStaffAssignmentDeletionNotificationAsync(Staff staff, Shift shift, Event @event)
    {
        var subject = $"Diensttoewijzing geannuleerd: {@event.Name}";
        var htmlBody = GenerateAssignmentDeletionEmailBody(staff, shift, @event);

        await SendEmailAsync(staff.Email, subject, htmlBody);
    }

    /// <summary>
    /// Sends an event planned notification email to the contact person
    /// </summary>
    public async Task SendEventPlannedNotificationAsync(Event @event)
    {
        if (string.IsNullOrEmpty(@event.ContactEmail))
        {
            _logger.LogWarning("Cannot send event planned email for event {EventId}: No contact email provided", @event.Id);
            return;
        }

        var subject = $"Update evenementplanning: {@event.Name}";
        var htmlBody = GenerateEventPlannedEmailBody(@event);
        
        await SendEmailAsync(@event.ContactEmail, subject, htmlBody);
    }

    /// <summary>
    /// Sends an event confirmation notification email to the contact person
    /// </summary>
    public async Task SendEventConfirmationNotificationAsync(Event @event)
    {
        if (string.IsNullOrEmpty(@event.ContactEmail))
        {
            _logger.LogWarning("Cannot send event confirmation email for event {EventId}: No contact email provided", @event.Id);
            return;
        }

        var subject = $"Evenement bevestigd: {@event.Name}";
        var htmlBody = GenerateEventConfirmationEmailBody(@event);
        
        await SendEmailAsync(@event.ContactEmail, subject, htmlBody);
    }

    /// <summary>
    /// Sends an invoice notification email to the contact person
    /// </summary>
    public async Task SendEventInvoiceNotificationAsync(Event @event)
    {
        if (string.IsNullOrEmpty(_emailOptions.Value.FinancialDepartmentEmail))
        {
            _logger.LogWarning("Cannot send event invoice email for event {EventId}: No financial email provided", @event.Id);
            return;
        }

        var subject = $"Factuur voor evenement: {@event.Name}";
        var htmlBody = GenerateEventInvoiceEmailBody(@event);
        
        await SendEmailAsync(_emailOptions.Value.FinancialDepartmentEmail, subject, htmlBody);
    }

    /// <summary>
    /// Sends a general email using SMTP
    /// </summary>
    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        try
        {
            var smtpConfig = _configuration.GetSection("EmailSettings");
            var host = smtpConfig["SmtpHost"];
            var port = int.Parse(smtpConfig["SmtpPort"] ?? "587");
            var username = smtpConfig["SmtpUsername"];
            var password = smtpConfig["SmtpPassword"];
            var fromEmail = smtpConfig["FromEmail"];
            var fromName = smtpConfig["FromName"] ?? "LOTUS Planning App";
            var enableSsl = bool.Parse(smtpConfig["EnableSsl"] ?? "true");

            if (string.IsNullOrEmpty(host)) // || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("Email settings not configured. Skipping email to {Email}", to);
                return;
            }

            using var client = new SmtpClient(host, port);

            if (_emailOptions.Value.EnableSsl)
            {
                client.EnableSsl = enableSsl;
                client.Credentials = new NetworkCredential(username, password);
            }
            else
            {
            }

            // Ensure we have a valid from address
            var fromAddress = fromEmail ?? username ?? "noreply@lotusapp.com";

            var message = new MailMessage
            {
                From = new MailAddress(fromAddress, fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            message.To.Add(to);

            await client.SendMailAsync(message);
            _logger.LogInformation("Email sent successfully to {Email}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", to);
            // Don't throw to prevent blocking the assignment creation
        }
    }

    /// <summary>
    /// Generates the HTML body for staff assignment notification email
    /// </summary>
    private string GenerateAssignmentEmailBody(Staff staff, Shift shift, Event @event)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine("    <meta charset='utf-8'>");
        sb.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
        sb.AppendLine("    <title>Diensttoewijzing</title>");
        sb.AppendLine("    <style>");
        sb.AppendLine("        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 20px; }");
        sb.AppendLine("        .container { max-width: 600px; margin: 0 auto; background: #f9f9f9; padding: 20px; border-radius: 8px; }");
        sb.AppendLine("        .header { background: #007bff; color: white; padding: 20px; border-radius: 8px 8px 0 0; text-align: center; }");
        sb.AppendLine("        .content { background: white; padding: 20px; border-radius: 0 0 8px 8px; }");
        sb.AppendLine("        .badge { display: inline-block; padding: 4px 8px; border-radius: 4px; font-size: 12px; font-weight: bold; }");
        sb.AppendLine("        .badge-success { background: #28a745; color: white; }");
        sb.AppendLine("        .badge-info { background: #17a2b8; color: white; }");
        sb.AppendLine("        .badge-warning { background: #ffc107; color: #212529; }");
        sb.AppendLine("        .badge-danger { background: #dc3545; color: white; }");
        sb.AppendLine("        .badge-primary { background: #007bff; color: white; }");
        sb.AppendLine("        .detail-row { margin: 10px 0; padding: 10px; background: #f8f9fa; border-radius: 4px; }");
        sb.AppendLine("        .detail-label { font-weight: bold; color: #495057; }");
        sb.AppendLine("        .footer { text-align: center; margin-top: 20px; font-size: 12px; color: #6c757d; }");
        sb.AppendLine("    </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("    <div class='container'>");
        sb.AppendLine("        <div class='header'>");
        sb.AppendLine("            <h1>?? Diensttoewijzing</h1>");
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class='content'>");
        
        sb.AppendLine($"            <p>Hallo <strong>{staff.FullName}</strong>,</p>");
        sb.AppendLine("            <p>Je bent toegewezen aan een nieuwe dienst. Bekijk hieronder de details:</p>");
        
        // Event Details
        sb.AppendLine("            <h3>?? Evenementdetails</h3>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Naam evenement:</div>");
        sb.AppendLine($"                <div>{@event.Name}</div>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Locatie:</div>");
        sb.AppendLine($"                <div>?? {@event.Location}</div>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Duur evenement:</div>");
        sb.AppendLine($"                <div>??? {@event.StartDate:g} - {@event.EndDate:g}</div>");
        sb.AppendLine("            </div>");
        
        if (!string.IsNullOrEmpty(@event.Description))
        {
            sb.AppendLine("            <div class='detail-row'>");
            sb.AppendLine($"                <div class='detail-label'>Omschrijving evenement:</div>");
            sb.AppendLine($"                <div>{@event.Description}</div>");
            sb.AppendLine("            </div>");
        }
        
        if (!string.IsNullOrEmpty(@event.ContactPerson))
        {
            sb.AppendLine("            <div class='detail-row'>");
            sb.AppendLine($"                <div class='detail-label'>Contactpersoon evenement:</div>");
            sb.AppendLine($"                <div>?? {@event.ContactPerson}");
            if (!string.IsNullOrEmpty(@event.ContactPhone))
            {
                sb.AppendLine($" - ?? {@event.ContactPhone}");
            }
            sb.AppendLine("</div>");
            sb.AppendLine("            </div>");
        }
        
        // Shift Details
        sb.AppendLine("            <h3>? Jouw dienstdetails</h3>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Naam dienst:</div>");
        sb.AppendLine($"                <div>{shift.Name}</div>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Diensttijd:</div>");
        sb.AppendLine($"                <div>?? {shift.StartTime:g} - {shift.EndTime:g}</div>");
        sb.AppendLine("            </div>");
        
        var duration = shift.EndTime - shift.StartTime;
        var durationText = duration.TotalHours >= 24 
            ? $"{duration.Days} d {duration.Hours} u"
            : $"{duration.Hours} u {duration.Minutes} min";
        
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Duur:</div>");
        sb.AppendLine($"                <div>?? {durationText}</div>");
        sb.AppendLine("            </div>");
        
        if (!string.IsNullOrEmpty(shift.Description))
        {
            sb.AppendLine("            <div class='detail-row'>");
            sb.AppendLine($"                <div class='detail-label'>Omschrijving dienst:</div>");
            sb.AppendLine($"                <div>{shift.Description}</div>");
            sb.AppendLine("            </div>");
        }
        
        // Important Notes
        sb.AppendLine("            <h3>?? Belangrijke opmerkingen</h3>");
        sb.AppendLine("            <ul>");
        sb.AppendLine("                <li>Kom voor de briefing <strong>15 minuten eerder</strong></li>");
        sb.AppendLine("                <li>Neem je certificaten en legitimatiebewijs mee</li>");
        sb.AppendLine("                <li>Draag passende medische/EHBO-kleding</li>");
        sb.AppendLine("                <li>Neem contact op met de organisator als je niet aanwezig kunt zijn</li>");
        sb.AppendLine("            </ul>");
        
        sb.AppendLine("            <p><strong>Bedankt voor je inzet!</strong></p>");
        sb.AppendLine("            <p>Neem bij vragen over deze toewijzing contact op met de organisator.</p>");
        
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class='footer'>");
        sb.AppendLine("            <p>Dit is een automatisch bericht van de LOTUS-planningsapp.</p>");
        sb.AppendLine($"            <p>E-mail verzonden op {DateTime.UtcNow:g} UTC</p>");
        sb.AppendLine("        </div>");
        sb.AppendLine("    </div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        
        return sb.ToString();
    }

    /// <summary>
    /// Generates the HTML body for staff assignment deletion notification email
    /// </summary>
    private string GenerateAssignmentDeletionEmailBody(Staff staff, Shift shift, Event @event)
    {
        var sb = new StringBuilder();

        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine("    <meta charset='utf-8'>");
        sb.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
        sb.AppendLine("    <title>Diensttoewijzing geannuleerd</title>");
        sb.AppendLine("    <style>");
        sb.AppendLine("        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 20px; }");
        sb.AppendLine("        .container { max-width: 600px; margin: 0 auto; background: #f9f9f9; padding: 20px; border-radius: 8px; }");
        sb.AppendLine("        .header { background: #dc3545; color: white; padding: 20px; border-radius: 8px 8px 0 0; text-align: center; }");
        sb.AppendLine("        .content { background: white; padding: 20px; border-radius: 0 0 8px 8px; }");
        sb.AppendLine("        .detail-row { margin: 10px 0; padding: 10px; background: #f8f9fa; border-radius: 4px; }");
        sb.AppendLine("        .detail-label { font-weight: bold; color: #495057; }");
        sb.AppendLine("        .footer { text-align: center; margin-top: 20px; font-size: 12px; color: #6c757d; }");
        sb.AppendLine("    </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("    <div class='container'>");
        sb.AppendLine("        <div class='header'>");
        sb.AppendLine("            <h1>Diensttoewijzing geannuleerd</h1>");
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class='content'>");

        sb.AppendLine($"            <p>Hallo <strong>{System.Net.WebUtility.HtmlEncode(staff.FullName)}</strong>,</p>");
        sb.AppendLine("            <p>Je toewijzing aan de volgende dienst is geannuleerd:</p>");

        sb.AppendLine("            <h3>Evenementdetails</h3>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Naam evenement:</div>");
        sb.AppendLine($"                <div>{System.Net.WebUtility.HtmlEncode(@event.Name)}</div>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Locatie:</div>");
        sb.AppendLine($"                <div>{System.Net.WebUtility.HtmlEncode(@event.Location)}</div>");
        sb.AppendLine("            </div>");

        sb.AppendLine("            <h3>Dienstdetails</h3>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Naam dienst:</div>");
        sb.AppendLine($"                <div>{System.Net.WebUtility.HtmlEncode(shift.Name)}</div>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Diensttijd:</div>");
        sb.AppendLine($"                <div>{shift.StartTime:g} - {shift.EndTime:g}</div>");
        sb.AppendLine("            </div>");

        sb.AppendLine("            <p>Neem contact op met de organisator als je denkt dat dit niet klopt.</p>");
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class='footer'>");
        sb.AppendLine("            <p>Dit is een automatisch bericht van de LOTUS-planningsapp.</p>");
        sb.AppendLine($"            <p>E-mail verzonden op {DateTime.UtcNow:g} UTC</p>");
        sb.AppendLine("        </div>");
        sb.AppendLine("    </div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }

    /// <summary>
    /// Generates the HTML body for event confirmation notification email
    /// </summary>
    private string GenerateEventConfirmationEmailBody(Event @event)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine("    <meta charset='utf-8'>");
        sb.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
        sb.AppendLine("    <title>Evenementbevestiging</title>");
        sb.AppendLine("    <style>");
        sb.AppendLine("        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 20px; }");
        sb.AppendLine("        .container { max-width: 600px; margin: 0 auto; background: #f9f9f9; padding: 20px; border-radius: 8px; }");
        sb.AppendLine("        .header { background: #28a745; color: white; padding: 20px; border-radius: 8px 8px 0 0; text-align: center; }");
        sb.AppendLine("        .content { background: white; padding: 20px; border-radius: 0 0 8px 8px; }");
        sb.AppendLine("        .badge { display: inline-block; padding: 4px 8px; border-radius: 4px; font-size: 12px; font-weight: bold; }");
        sb.AppendLine("        .badge-success { background: #28a745; color: white; }");
        sb.AppendLine("        .detail-row { margin: 10px 0; padding: 10px; background: #f8f9fa; border-radius: 4px; }");
        sb.AppendLine("        .detail-label { font-weight: bold; color: #495057; }");
        sb.AppendLine("        .footer { text-align: center; margin-top: 20px; font-size: 12px; color: #6c757d; }");
        sb.AppendLine("        .highlight { background: #fff3cd; padding: 15px; border-radius: 4px; border-left: 4px solid #ffc107; margin: 15px 0; }");
        sb.AppendLine("    </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("    <div class='container'>");
        sb.AppendLine("        <div class='header'>");
        sb.AppendLine("            <h1>? Evenementbevestiging</h1>");
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class='content'>");
        
        if (!string.IsNullOrEmpty(@event.ContactPerson))
        {
            sb.AppendLine($"            <p>Hallo <strong>{@event.ContactPerson}</strong>,</p>");
        }
        else
        {
            sb.AppendLine("            <p>Hallo,</p>");
        }
        
        sb.AppendLine("            <p>Goed nieuws! Je aanvraag voor het evenement is <strong>bevestigd</strong> door ons LOTUS-team.</p>");
        
        sb.AppendLine("            <div class='highlight'>");
        sb.AppendLine("                <h3>?? Je evenement is bevestigd!</h3>");
        sb.AppendLine("                <p>We bevestigen graag dat de medische hulpverlening voor je evenement is geregeld.</p>");
        sb.AppendLine("            </div>");
        
        // Event Details
        sb.AppendLine("            <h3>?? Evenementdetails</h3>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Naam evenement:</div>");
        sb.AppendLine($"                <div>{@event.Name}</div>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Locatie:</div>");
        sb.AppendLine($"                <div>?? {@event.Location}</div>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Duur evenement:</div>");
        sb.AppendLine($"                <div>??? {@event.StartDate:g} - {@event.EndDate:g}</div>");
        sb.AppendLine("            </div>");
        
        var duration = @event.EndDate - @event.StartDate;
        var durationText = duration.TotalDays >= 1 
            ? $"{duration.Days} dag(en), {duration.Hours} uur"
            : $"{duration.Hours} uur, {duration.Minutes} minuten";
        
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Duur:</div>");
        sb.AppendLine($"                <div>?? {durationText}</div>");
        sb.AppendLine("            </div>");
        
        if (!string.IsNullOrEmpty(@event.Description))
        {
            sb.AppendLine("            <div class='detail-row'>");
            sb.AppendLine($"                <div class='detail-label'>Omschrijving evenement:</div>");
            sb.AppendLine($"                <div>{@event.Description}</div>");
            sb.AppendLine("            </div>");
        }
        
        // Status Badge
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Status:</div>");
        sb.AppendLine($"                <div><span class='badge badge-success'>? Bevestigd</span></div>");
        sb.AppendLine("            </div>");
        
        // What's Next Section
        sb.AppendLine("            <h3>?? Hoe gaat het verder?</h3>");
        sb.AppendLine("            <ul>");
        sb.AppendLine("                <li><strong>Personeelstoewijzing:</strong> ons team wijst gekwalificeerd personeel toe aan je evenement</li>");
        sb.AppendLine("                <li><strong>Contact vooraf:</strong> we nemen 24-48 uur voor het evenement contact met je op</li>");
        sb.AppendLine("                <li><strong>Dag van het evenement:</strong> ons personeel arriveert 30 minuten eerder voor de opbouw en briefing</li>");
        sb.AppendLine("                <li><strong>Materialen:</strong> alle benodigde medische materialen worden verzorgd</li>");
        sb.AppendLine("            </ul>");
        
        // Important Notes
        sb.AppendLine("            <h3>?? Belangrijke informatie</h3>");
        sb.AppendLine("            <ul>");
        sb.AppendLine("                <li>Zorg voor voldoende parkeergelegenheid en toegang voor ons team</li>");
        sb.AppendLine("                <li>Laat het ons direct weten als evenementdetails wijzigen</li>");
        sb.AppendLine("                <li>Ons team heeft toegang tot stopcontacten voor medische apparatuur nodig</li>");
        sb.AppendLine("                <li>Er moet een geschikte plek voor de EHBO-post beschikbaar zijn</li>");
        sb.AppendLine("            </ul>");
        
        // Contact Information
        sb.AppendLine("            <h3>?? Wil je iets wijzigen?</h3>");
        sb.AppendLine("            <p>Neem zo snel mogelijk contact met ons op als je iets aan je evenement wilt wijzigen of vragen hebt:</p>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine("                <div>?? E-mail: <a href='mailto:events@medicalfirstaid.com'>events@medicalfirstaid.com</a></div>");
        sb.AppendLine("                <div>?? Telefoon: +1 (555) 123-4567</div>");
        sb.AppendLine("                <div>?? Openingstijden: maandag-vrijdag, 08:00 - 18:00 uur</div>");
        sb.AppendLine("            </div>");
        
        sb.AppendLine("            <p><strong>Bedankt dat je voor onze medische hulpverlening kiest!</strong></p>");
        sb.AppendLine("            <p>We kijken ernaar uit om professionele medische hulpverlening voor je evenement te verzorgen.</p>");
        
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class='footer'>");
        sb.AppendLine("            <p>Dit is een automatische bevestiging van de LOTUS-planningsapp.</p>");
        sb.AppendLine($"            <p>Bevestiging verzonden op {DateTime.UtcNow:g} UTC</p>");
        sb.AppendLine($"            <p>Referentie-ID: EVENT-{@event.Id:D6}</p>");
        sb.AppendLine("        </div>");
        sb.AppendLine("    </div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        
        return sb.ToString();
    }

    /// <summary>
    /// Generates the HTML body for event planned notification email
    /// </summary>
    private string GenerateEventPlannedEmailBody(Event @event)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine("    <meta charset='utf-8'>");
        sb.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
        sb.AppendLine("    <title>Update evenementplanning</title>");
        sb.AppendLine("    <style>");
        sb.AppendLine("        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 20px; }");
        sb.AppendLine("        .container { max-width: 600px; margin: 0 auto; background: #f9f9f9; padding: 20px; border-radius: 8px; }");
        sb.AppendLine("        .header { background: #007bff; color: white; padding: 20px; border-radius: 8px 8px 0 0; text-align: center; }");
        sb.AppendLine("        .content { background: white; padding: 20px; border-radius: 0 0 8px 8px; }");
        sb.AppendLine("        .badge { display: inline-block; padding: 4px 8px; border-radius: 4px; font-size: 12px; font-weight: bold; }");
        sb.AppendLine("        .badge-primary { background: #007bff; color: white; }");
        sb.AppendLine("        .detail-row { margin: 10px 0; padding: 10px; background: #f8f9fa; border-radius: 4px; }");
        sb.AppendLine("        .detail-label { font-weight: bold; color: #495057; }");
        sb.AppendLine("        .footer { text-align: center; margin-top: 20px; font-size: 12px; color: #6c757d; }");
        sb.AppendLine("        .highlight { background: #d1ecf1; padding: 15px; border-radius: 4px; border-left: 4px solid #007bff; margin: 15px 0; }");
        sb.AppendLine("    </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("    <div class='container'>");
        sb.AppendLine("        <div class='header'>");
        sb.AppendLine("            <h1>?? Update evenementplanning</h1>");
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class='content'>");
        
        if (!string.IsNullOrEmpty(@event.ContactPerson))
        {
            sb.AppendLine($"            <p>Hallo <strong>{@event.ContactPerson}</strong>,</p>");
        }
        else
        {
            sb.AppendLine("            <p>Hallo,</p>");
        }
        
        sb.AppendLine("            <p>Goed nieuws! We zijn gestart met de planning van de medische hulpverlening voor je evenement.</p>");
        
        sb.AppendLine("            <div class='highlight'>");
        sb.AppendLine("                <h3>?? Je evenement wordt nu gepland</h3>");
        sb.AppendLine("                <p>Ons team heeft je aanvraag beoordeeld en werkt nu aan:</p>");
        sb.AppendLine("                <ul>");
        sb.AppendLine("                    <li>het bepalen van de benodigde medische hulpverlening</li>");
        sb.AppendLine("                    <li>het inplannen van geschikt medisch personeel</li>");
        sb.AppendLine("                    <li>het voorbereiden van benodigde medische apparatuur en materialen</li>");
        sb.AppendLine("                    <li>het plannen van de EHBO-post</li>");
        sb.AppendLine("                </ul>");
        sb.AppendLine("            </div>");
        
        // Event Details
        sb.AppendLine("            <h3>?? Evenementdetails</h3>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Naam evenement:</div>");
        sb.AppendLine($"                <div>{@event.Name}</div>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Locatie:</div>");
        sb.AppendLine($"                <div>?? {@event.Location}</div>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Duur evenement:</div>");
        sb.AppendLine($"                <div>?? {@event.StartDate:g} - {@event.EndDate:g}</div>");
        sb.AppendLine("            </div>");
        
        var duration = @event.EndDate - @event.StartDate;
        var durationText = duration.TotalDays >= 1 
            ? $"{duration.Days} dag(en), {duration.Hours} uur"
            : $"{duration.Hours} uur, {duration.Minutes} minuten";
        
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Duur:</div>");
        sb.AppendLine($"                <div>?? {durationText}</div>");
        sb.AppendLine("            </div>");
        
        if (!string.IsNullOrEmpty(@event.Description))
        {
            sb.AppendLine("            <div class='detail-row'>");
            sb.AppendLine($"                <div class='detail-label'>Omschrijving evenement:</div>");
            sb.AppendLine($"                <div>{@event.Description}</div>");
            sb.AppendLine("            </div>");
        }
        
        // Status Badge
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Huidige status:</div>");
        sb.AppendLine($"                <div><span class='badge badge-primary'>?? Ingepland</span></div>");
        sb.AppendLine("            </div>");
        
        // What's Next Section
        sb.AppendLine("            <h3>?? Hoe gaat het verder?</h3>");
        sb.AppendLine("            <ul>");
        sb.AppendLine("                <li><strong>Planningsfase:</strong> ons team rondt het personeels- en hulpverleningsplan af</li>");
        sb.AppendLine("                <li><strong>Personeelstoewijzing:</strong> gekwalificeerd medisch personeel wordt toegewezen</li>");
        sb.AppendLine("                <li><strong>Definitieve bevestiging:</strong> je ontvangt een bevestiging zodra alles geregeld is</li>");
        sb.AppendLine("                <li><strong>Contact vooraf:</strong> we nemen 24-48 uur voor het evenement contact met je op</li>");
        sb.AppendLine("            </ul>");
        
        // Important Information
        sb.AppendLine("            <h3>?? Belangrijke informatie</h3>");
        sb.AppendLine("            <p>Zorg tijdens de planning voor het volgende:</p>");
        sb.AppendLine("            <ul>");
        sb.AppendLine("                <li>Evenementdetails blijven actueel; geef wijzigingen direct door</li>");
        sb.AppendLine("                <li>Er is voldoende parkeergelegenheid en toegang voor ons team</li>");
        sb.AppendLine("                <li>Stopcontacten zijn toegankelijk voor medische apparatuur</li>");
        sb.AppendLine("                <li>Een geschikte plek voor de EHBO-post kan worden aangewezen</li>");
        sb.AppendLine("            </ul>");
        
        // Estimated Timeline
        sb.AppendLine("            <h3>? Verwachte planning</h3>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine("                <div>?? <strong>Planningsfase:</strong> momenteel bezig</div>");
        sb.AppendLine("                <div>? <strong>Bevestiging:</strong> binnen 2-3 werkdagen</div>");
        sb.AppendLine("                <div>?? <strong>Contact vooraf:</strong> 24-48 uur voor het evenement</div>");
        sb.AppendLine("            </div>");
        
        // Contact Information
        sb.AppendLine("            <h3>?? Wil je iets wijzigen of heb je vragen?</h3>");
        sb.AppendLine("            <p>Neem direct contact met ons op als je iets aan je evenement wilt wijzigen of vragen hebt over de planning:</p>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine("                <div>?? E-mail: <a href='mailto:events@medicalfirstaid.com'>events@medicalfirstaid.com</a></div>");
        sb.AppendLine("                <div>?? Telefoon: +1 (555) 123-4567</div>");
        sb.AppendLine("                <div>?? Openingstijden: maandag-vrijdag, 08:00 - 18:00 uur</div>");
        sb.AppendLine("            </div>");
        
        sb.AppendLine("            <p><strong>Bedankt dat je voor onze medische hulpverlening kiest!</strong></p>");
        sb.AppendLine("            <p>We zetten ons in voor professionele en betrouwbare medische hulpverlening voor je evenement.</p>");
        
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class='footer'>");
        sb.AppendLine("            <p>Dit is een automatisch bericht van de LOTUS-planningsapp.</p>");
        sb.AppendLine($"            <p>Planningsbericht verzonden op {DateTime.UtcNow:g} UTC</p>");
        sb.AppendLine($"            <p>Referentie-ID: EVENT-{@event.Id:D6}</p>");
        sb.AppendLine("        </div>");
        sb.AppendLine("    </div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        
        return sb.ToString();
    }

    /// <summary>
    /// Generates the HTML body for event invoice notification email
    /// </summary>
    private string GenerateEventInvoiceEmailBody(Event @event)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine("    <meta charset='utf-8'>");
        sb.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
        sb.AppendLine("    <title>Factuur evenement</title>");
        sb.AppendLine("    <style>");
        sb.AppendLine("        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 20px; }");
        sb.AppendLine("        .container { max-width: 600px; margin: 0 auto; background: #f9f9f9; padding: 20px; border-radius: 8px; }");
        sb.AppendLine("        .header { background: #6f42c1; color: white; padding: 20px; border-radius: 8px 8px 0 0; text-align: center; }");
        sb.AppendLine("        .content { background: white; padding: 20px; border-radius: 0 0 8px 8px; }");
        sb.AppendLine("        .badge { display: inline-block; padding: 4px 8px; border-radius: 4px; font-size: 12px; font-weight: bold; }");
        sb.AppendLine("        .badge-primary { background: #6f42c1; color: white; }");
        sb.AppendLine("        .detail-row { margin: 10px 0; padding: 10px; background: #f8f9fa; border-radius: 4px; }");
        sb.AppendLine("        .detail-label { font-weight: bold; color: #495057; }");
        sb.AppendLine("        .footer { text-align: center; margin-top: 20px; font-size: 12px; color: #6c757d; }");
        sb.AppendLine("        .highlight { background: #e7d3ff; padding: 15px; border-radius: 4px; border-left: 4px solid #6f42c1; margin: 15px 0; }");
        sb.AppendLine("        .invoice-info { background: #f8f9fa; padding: 15px; border-radius: 4px; margin: 15px 0; }");
        sb.AppendLine("        .amount { font-size: 1.2em; font-weight: bold; color: #6f42c1; }");
        sb.AppendLine("    </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("    <div class='container'>");
        sb.AppendLine("        <div class='header'>");
        sb.AppendLine("            <h1>?? Factuur evenement</h1>");
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class='content'>");
        
        if (!string.IsNullOrEmpty(@event.ContactPerson))
        {
            sb.AppendLine($"            <p>Beste <strong>{@event.ContactPerson}</strong>,</p>");
        }
        else
        {
            sb.AppendLine("            <p>Beste klant,</p>");
        }
        
        sb.AppendLine("            <p>Bedankt dat je voor onze medische hulpverlening hebt gekozen. Je evenement is afgerond en hierbij ontvang je de factuur voor de geleverde diensten.</p>");
        
        sb.AppendLine("            <div class='highlight'>");
        sb.AppendLine("                <h3>?? Factuur voor je evenement</h3>");
        sb.AppendLine("                <p>Ons LOTUS-team heeft de medische hulpverlening voor je evenement verzorgd. Hieronder staan de factuurgegevens en onze betalingsvoorwaarden.</p>");
        sb.AppendLine("            </div>");
        
        // Event Details
        sb.AppendLine("            <h3>?? Evenementdetails</h3>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Naam evenement:</div>");
        sb.AppendLine($"                <div>{@event.Name}</div>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Locatie:</div>");
        sb.AppendLine($"                <div>?? {@event.Location}</div>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Datum en tijd evenement:</div>");
        sb.AppendLine($"                <div>??? {@event.StartDate:g} - {@event.EndDate:g}</div>");
        sb.AppendLine("            </div>");
        
        var duration = @event.EndDate - @event.StartDate;
        var durationText = duration.TotalDays >= 1 
            ? $"{duration.Days} dag(en), {duration.Hours} uur"
            : $"{duration.Hours} uur, {duration.Minutes} minuten";
        
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Duur evenement:</div>");
        sb.AppendLine($"                <div>?? {durationText}</div>");
        sb.AppendLine("            </div>");
        
        if (!string.IsNullOrEmpty(@event.Description))
        {
            sb.AppendLine("            <div class='detail-row'>");
            sb.AppendLine($"                <div class='detail-label'>Omschrijving evenement:</div>");
            sb.AppendLine($"                <div>{@event.Description}</div>");
            sb.AppendLine("            </div>");
        }
        
        // Status Badge
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Status evenement:</div>");
        sb.AppendLine($"                <div><span class='badge badge-primary'>?? Factuur verzonden</span></div>");
        sb.AppendLine("            </div>");
        
        // Invoice Information
        sb.AppendLine("            <h3>?? Factuurgegevens</h3>");
        sb.AppendLine("            <div class='invoice-info'>");
        sb.AppendLine($"                <div class='detail-row'>");
        sb.AppendLine($"                    <div class='detail-label'>Factuurnummer:</div>");
        sb.AppendLine($"                    <div>INV-{@event.Id:D6}-{DateTime.UtcNow:yyyyMM}</div>");
        sb.AppendLine("                </div>");
        sb.AppendLine($"                <div class='detail-row'>");
        sb.AppendLine($"                    <div class='detail-label'>Factuurdatum:</div>");
        sb.AppendLine($"                    <div>{DateTime.UtcNow:d}</div>");
        sb.AppendLine("                </div>");
        sb.AppendLine($"                <div class='detail-row'>");
        sb.AppendLine($"                    <div class='detail-label'>Periode:</div>");
        sb.AppendLine($"                    <div>{@event.StartDate:d} - {@event.EndDate:d}</div>");
        sb.AppendLine("                </div>");
        sb.AppendLine("            </div>");
        
        // Services Provided Section
        sb.AppendLine("            <h3>?? Geleverde diensten</h3>");
        sb.AppendLine("            <ul>");
        sb.AppendLine("                <li><strong>Medische hulpverlening:</strong> professioneel medisch personeel aanwezig tijdens het evenement</li>");
        sb.AppendLine("                <li><strong>Noodhulp:</strong> directe medische hulp bij incidenten</li>");
        sb.AppendLine("                <li><strong>Medische apparatuur:</strong> volledige EHBO-set en medische noodmaterialen</li>");
        sb.AppendLine("                <li><strong>Deskundigheid personeel:</strong> gecertificeerde medische professionals en hulpverleners</li>");
        sb.AppendLine("                <li><strong>Documentatie:</strong> incidentrapporten en medische registraties indien nodig</li>");
        sb.AppendLine("            </ul>");
        
        // Payment Instructions
        sb.AppendLine("            <h3>?? Betalingsinformatie</h3>");
        sb.AppendLine("            <div class='highlight'>");
        sb.AppendLine("                <p><strong>Betalingsvoorwaarden:</strong> binnen 30 dagen na factuurdatum</p>");
        sb.AppendLine("                <p><strong>Vervaldatum:</strong> " + DateTime.UtcNow.AddDays(30).ToString("d") + "</p>");
        sb.AppendLine("            </div>");
        
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine("                <div><strong>Geaccepteerde betaalmethoden:</strong></div>");
        sb.AppendLine("                <div>");
        sb.AppendLine("                    <ul>");
        sb.AppendLine("                        <li>?? Creditcard (Visa, MasterCard, AmEx)</li>");
        sb.AppendLine("                        <li>?? Bankoverschrijving</li>");
        sb.AppendLine("                        <li>?? Cheque (ten name van Medical First Aid Services)</li>");
        sb.AppendLine("                        <li>?? Online betaalportaal</li>");
        sb.AppendLine("                    </ul>");
        sb.AppendLine("                </div>");
        sb.AppendLine("            </div>");
        
        // Contact Information for Invoice Queries
        sb.AppendLine("            <h3>?? Vragen over de factuur?</h3>");
        sb.AppendLine("            <p>Neem contact op met onze facturatieafdeling als je vragen hebt over deze factuur of de geleverde diensten:</p>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine("                <div>?? E-mail: <a href='mailto:billing@medicalfirstaid.com'>billing@medicalfirstaid.com</a></div>");
        sb.AppendLine("                <div>?? Telefoon: +1 (555) 123-4567, toestel 2</div>");
        sb.AppendLine("                <div>?? Openingstijden facturatie: maandag-vrijdag, 09:00 - 17:00 uur</div>");
        sb.AppendLine("            </div>");
        
        // Additional Notes
        sb.AppendLine("            <h3>?? Belangrijke opmerkingen</h3>");
        sb.AppendLine("            <ul>");
        sb.AppendLine("                <li>Vermeld het factuurnummer bij alle betalingen</li>");
        sb.AppendLine("                <li>Na de vervaldatum kunnen kosten voor te late betaling gelden</li>");
        sb.AppendLine("                <li>Neem voor terugkerende evenementen contact met ons op over volumekorting</li>");
        sb.AppendLine("                <li>Bedankt voor je vertrouwen; we helpen je graag weer bij een volgend evenement</li>");
        sb.AppendLine("            </ul>");
        
        sb.AppendLine("            <p><strong>Bedankt dat je voor onze medische hulpverlening kiest!</strong></p>");
        sb.AppendLine("            <p>We waarderen je vertrouwen en hopen dat je tevreden bent over onze professionele medische hulpverlening. Neem gerust contact met ons op voor toekomstige evenementen.</p>");
        
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class='footer'>");
        sb.AppendLine("            <p>Deze factuur is automatisch gegenereerd door de LOTUS-planningsapp.</p>");
        sb.AppendLine($"            <p>Factuur verzonden op {DateTime.UtcNow:g} UTC</p>");
        sb.AppendLine($"            <p>Referentie-ID: EVENT-{@event.Id:D6} | Factuur: INV-{@event.Id:D6}-{DateTime.UtcNow:yyyyMM}</p>");
        sb.AppendLine("        </div>");
        sb.AppendLine("    </div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        
        return sb.ToString();
    }
}