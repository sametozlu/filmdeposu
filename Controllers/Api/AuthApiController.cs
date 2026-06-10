using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FilmSerileri.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

namespace FilmSerileri.Controllers.Api;

public record TokenRequest(string Email, string Password);

[ApiController]
[Route("api/v1/auth")]
[EnableRateLimiting("api")]
public class AuthApiController : ControllerBase
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IConfiguration _config;

  public AuthApiController(UserManager<ApplicationUser> userManager, IConfiguration config)
  {
    _userManager = userManager;
    _config = config;
  }

  /// <summary>E-posta + şifre ile JWT alın; korumalı API uçlarında Bearer token olarak kullanın.</summary>
  [HttpPost("token")]
  public async Task<IActionResult> Token([FromBody] TokenRequest request)
  {
    var user = await _userManager.FindByEmailAsync(request.Email);
    if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
      return Unauthorized(new { error = "Invalid credentials" });

    var roles = await _userManager.GetRolesAsync(user);
    var claims = new List<Claim>
    {
      new(JwtRegisteredClaimNames.Sub, user.Id),
      new(ClaimTypes.NameIdentifier, user.Id),
      new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
      new(ClaimTypes.Name, user.UserName ?? string.Empty),
      new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };
    claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
    var expires = DateTime.UtcNow.AddDays(7);

    var token = new JwtSecurityToken(
      issuer: _config["Jwt:Issuer"],
      audience: _config["Jwt:Audience"],
      claims: claims,
      expires: expires,
      signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

    return Ok(new
    {
      accessToken = new JwtSecurityTokenHandler().WriteToken(token),
      tokenType = "Bearer",
      expiresAt = expires
    });
  }
}
