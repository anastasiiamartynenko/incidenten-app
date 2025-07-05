using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Incidenten.API.Interfaces;
using Incidenten.Domain;
using Incidenten.Domain.Enums;
using Incidenten.Infrastructures;
using Incidenten.Shared.DTO.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Incidenten.API.Services;

public class UserService : IUserService
{
    private readonly IncidentenDbContext _db;
    private readonly string _employeeEnding;
    private readonly List<string> _officialEmails;
    private readonly string _anonymId;
    private readonly byte[] _jwtKey;

    public UserService(IncidentenDbContext db, IConfiguration configuration)
    {
        _db = db;
        
        // Get the data needed for defining the user's role from the configuration.
        _employeeEnding = configuration["Email:EmployeeEmailEnding"] ?? "@gmail.com";
        _officialEmails = configuration.GetSection("Email:OfficialEmails").Get<List<string>>() ?? [];
        _anonymId = configuration["Utils:AnonymId"] 
                   ?? throw new Exception("Anonym ID is missing in the configuration file.");
        
        // Get the JWT key from the configuration.
        _jwtKey = Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new Exception("No Jwt:Key found in the config."));
    }

    /**
     * A helper function to get the user by email.
     */
    public async Task<User?> GetUserByEmail(string? email)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    /**
     * Get anonym user from DB.
     */
    public async Task<User?> GetAnonymUser()
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(_anonymId));
    }

    /**
     * Check whether the user email is unique.
     */
    public bool IsUserEmailUnique(string? email)
    {
        return !_db.Users.Any(x => x.Email == email);
    }

    public bool IsPasswordValid(User? user, string password)
    {
        return user != null && BCrypt.Net.BCrypt.Verify(password, user.Password);
    }
    
    /**
     * Returns true if user has a role Employee or Official.
     */
    public bool IsPrivelegedRole(User user)
    {
        return user.Role == UserRole.Employee || user.Role == UserRole.Official;
    }
    /**
     * Finds user by email and return true if user has a role Employee or Official.
     */
    public async Task<bool> IsPrivelegedRoleAsync(string email)
    {
        var user = await GetUserByEmail(email);
        if (user == null) return false;
        return IsPrivelegedRole(user);
    }
    
    /**
     * Returns the role of the user based on the email provided.
     * The algorithm can be enhanced in the future.
     */
    public UserRole GetUserRole(string email)
    {
        if (_officialEmails.Contains(email, StringComparer.OrdinalIgnoreCase))
        {
            return UserRole.Official;
        }

        if (email.EndsWith(_employeeEnding, StringComparison.OrdinalIgnoreCase))
        {
            return UserRole.Employee;
        }

        // By default, the authenticated user's role is Citizen.
        return UserRole.Citizen;
    }

    /**
     * Generate the token.
     */
    public string GenerateToken(User user)
    {
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
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_jwtKey),
                SecurityAlgorithms.HmacSha256Signature));
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /**
     * Create the user and persist the changes to DB.
     */
    public async Task<User> CreateUser(SignUpRequest request)
    {
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
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return user;
    }

    /**
     * Toggle user's SendNotifications choice and persist the changes to DB.
     */
    public async Task ToggleUserNotifications(User user)
    {
        user.SendNotifications = !user.SendNotifications;
        await _db.SaveChangesAsync();
    }
}