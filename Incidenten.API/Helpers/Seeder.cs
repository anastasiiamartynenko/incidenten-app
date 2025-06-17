using System.Reflection;
using System.Text.RegularExpressions;
using Incidenten.Domain;
using Incidenten.Infrastructures;
using Microsoft.EntityFrameworkCore;

public static class Seeder
{
    private static string FormatEmailName(string fileName)
    {
        return Regex.Replace(fileName, "([a-z])([A-Z])", "$1 $2")
            .Replace("_", " ")
            .Trim();
    }


    public static async Task SeedEmails(IncidentenDbContext db)
    {
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "MailTemplates");
        if (!Directory.Exists(folderPath))
            return;

        var files = Directory.GetFiles(folderPath, "*.html");

        foreach (var filePath in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var formattedName = FormatEmailName(fileName);

            var emailBody = await File.ReadAllTextAsync(filePath);

            if (!await db.EmailTemplates.AnyAsync(e => e.Subject == formattedName))
            {
                var emailTemplate = new EmailTemplate
                {
                    Name = fileName,
                    Subject = formattedName,
                    Body = emailBody
                };

                await db.EmailTemplates.AddAsync(emailTemplate);
            }
        }

        await db.SaveChangesAsync();
    }
}