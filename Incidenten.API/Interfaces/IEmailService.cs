using Incidenten.Domain;

namespace Incidenten.API.Interfaces;

public interface IEmailService
{
    void SendMail(string to, EmailTemplate template, Dictionary<string, string>? placeholders);
}