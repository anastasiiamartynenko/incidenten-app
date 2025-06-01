using System.Net.Mail;

namespace Incidenten.Shared.Utils;

public class ValidationHelper
{
    /**
     * Returns true if the email is valid.
     */
    public bool IsValidEmail(string email)
    {
        try
        {
            var m = new MailAddress(email);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    /**
     * Returns true if the string contains at least 1 special character.
     */
    private bool ContainsSpecialCharacters(string str)
    {
        string specialChars = @"!@#$%^&*()_+-=<>,./|\";
        char[] chars = specialChars.ToCharArray();
        foreach (char c in chars)
        {
            if (str.Contains(c)) return true;
        }
        
        return false;
    }

    /**
     * Returns true if the password is valid.
     */
    public bool IsValidPassword(string password)
    {
        if (password.Length < 8 
            || !password.Any(char.IsUpper) 
            || !password.Any(char.IsLower) 
            || !ContainsSpecialCharacters(password))
            return false;
        return true;
    }

    /**
     * Returns true if the full name is valid.
     */
    public bool IsNotBlank(string value)
    {
        if (string.IsNullOrEmpty(value))
            return false;
        return true;
    }
}