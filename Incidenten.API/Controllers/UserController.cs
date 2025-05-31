using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Incidenten.Domain;
using Incidenten.Domain.Enums;
using Incidenten.Infrastructures;
using Incidenten.Shared.DTO.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Incidenten.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IncidentenDbContext db, IConfiguration config) : Controller
{
    /**
     * Returns the role of the user based on the email provided.
     * The algorithm can be enhanced in the future.
     */
    private UserRole GetUserRole(string email)
    {
        // Get the data needed for defining the user's role from the configuration.
        var employeeEnding = config["Email:EmployeeEmailEnding"] ?? "@gmail.com";
        var officialEmails = config.GetSection("Email:OfficialEmails").Get<List<string>>() ?? [];
        
        if (officialEmails.Contains(email, StringComparer.OrdinalIgnoreCase))
        {
            return UserRole.Official;
        }

        if (email.EndsWith(employeeEnding, StringComparison.OrdinalIgnoreCase))
        {
            return UserRole.Employee;
        }

        // By default, the authenticated user's role is Citizen.
        return UserRole.Citizen;
    }

    /**
     * Generate the token.
     */
    private string GenerateToken(User user)
    {
        // Get the JWT key from the configuration.
        var jwtKey = Encoding.UTF8.GetBytes(config["Jwt:Key"] 
                                            ?? throw new Exception("No Jwt:Key found in the config."));

        // Include user's email and role in the claims.
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim("role", user.Role.ToString()),
        };

        // Generate the token.
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(14),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(jwtKey),
                SecurityAlgorithms.HmacSha256Signature));
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /**
     * Sign up the user.
     */
    [HttpPost("sign-up")]
    public async Task<IActionResult> Signup([FromBody] SignUpRequest request)
    {
        // Make sure the email is not yet in use.
        if (db.Users.Any(x => x.Email == request.Email))
        {
            return Conflict(new { message = $"User with the email \"{request.Email}\" already exists." });
        }

        // Get user's role and hashed password.
        var userRole = GetUserRole(request.Email);
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Create the user.
        var user = new User
        {
            Email = request.Email,
            Password = hashedPassword,
            FullName = request.FullName,
            SendNotifications = request.SendNotifications,
            Role = userRole,
        };

        // Persist the changes in the DB.
        db.Users.Add(user);
        await db.SaveChangesAsync();

        // Generate and return the token for the user.
        var token = GenerateToken(user);
        return Ok(new SignUpResponse
        {
            Token = token,
        });
    }

    /**
     * Log in the user.
     */
    [HttpPost("log-in")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Make sure the user exists in the DB.
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        // Make sure the provided password is valid.
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            return Unauthorized();

        // Generate and return the token for the user.
        var jwtKey = GenerateToken(user);
        return Ok(new LogInResponse { Token = jwtKey });
    }

    /**
     * Get user's additional information.
     */
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        // Get the email from the token.
        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(email))
            return Unauthorized();
        
        // Get the user's additional info from the DB and return it in case it is found.
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            return NotFound();
        
        return Ok(user);
    }
}