using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Incidenten.API.Services;
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
public class UserController(UserService userService) : Controller
{
    /**
     * Sign up the user.
     */
    [HttpPost("sign-up")]
    public async Task<IActionResult> Signup([FromBody] SignUpRequest request)
    {
        // Make sure the email is not yet in use.
        if (!userService.IsUserEmailUnique(request.Email))
            return Conflict(new { message = $"User with the email \"{request.Email}\" already exists." });

        var user = await userService.CreateUser(request);

        // Generate and return the token for the user.
        var token = userService.GenerateToken(user);
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
        var user = await userService.GetUserByEmail(request.Email);
        // Make sure the provided password is valid.
        if (user == null || !userService.IsPasswordValid(user, request.Password))
            return Unauthorized();

        // Generate and return the token for the user.
        var jwtKey = userService.GenerateToken(user);
        return Ok(new LogInResponse { Token = jwtKey });
    }

    /**
     * Get user's additional information.
     */
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        // Get the user's additional info from the DB and return it in case it is found.
        var user = await userService.GetUserByEmail(User.Identity?.Name);
        if (user == null)
            return Unauthorized();
        
        return Ok(user);
    }

    [Authorize]
    [HttpPut("notifications")]
    public async Task<IActionResult> ToggleNotifications()
    {
        var user = await userService.GetUserByEmail(User.Identity?.Name ?? string.Empty);
        if (user == null) return Unauthorized();

        await userService.ToggleUserNotifications(user);
        
        return Ok();
    }
}