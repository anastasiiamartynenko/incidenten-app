using Incidenten.API.Interfaces;
using Incidenten.Domain;
using Incidenten.Infrastructures;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using MimeKit;

public class EmailService : IEmailService
{
    private readonly IncidentenDbContext _db;
    private readonly string host = string.Empty;
    private readonly int port = 587;
    private readonly string username = string.Empty;
    private readonly string password = string.Empty;
    private readonly string _updateStatusTemplateName;

    public EmailService(IncidentenDbContext db, IConfiguration config)
    {
        _db = db;
        _updateStatusTemplateName = config["Email:UpdateStatusTemplateName"] ?? throw new Exception("Email template \"Update status\" not found.");
        
        var emailNode = config.GetSection("Email");

        if (emailNode.Exists())
        {
            host = emailNode.GetValue<string>("Host") ?? string.Empty;
            port = emailNode.GetValue<int>("Port");
            username = emailNode.GetValue<string>("Username") ?? string.Empty;
            password = emailNode.GetValue<string>("Password") ?? string.Empty;
        }
    }
    
    public async Task<EmailTemplate?> GetUpdateStatusTemplate()
    {
        return  await _db.EmailTemplates.FirstOrDefaultAsync(t => t.Name == _updateStatusTemplateName);
    }
    
    private string ReplacePlaceholders(string initialString, Dictionary<string, string> placeholders)
    {
        string body = initialString;
        foreach (var kvp in placeholders)
        {
            body = body.Replace("{{" + kvp.Key + "}}", kvp.Value);
        }
        return body;
    }

    public void SendMail(string to, EmailTemplate template, Dictionary<string, string>? placeholders = null)
    {
        using var smtp = new SmtpClient();
        smtp.Connect(host, port, MailKit.Security.SecureSocketOptions.StartTls);
        smtp.Authenticate(
            username,
            password
        );

        var email = new MimeMessage();

        email.To.Add(MailboxAddress.Parse(to));

        email.From.Add(MailboxAddress.Parse(username));
        email.Subject = template.Subject;

        string body = template.Body;
        if (placeholders != null)
        {
            body = ReplacePlaceholders(body, placeholders);
        }
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };


        smtp.Send(email);

        smtp.Disconnect(true);
    }
}