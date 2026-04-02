using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend_purchase_order.Models;
using backend_purchase_order.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace backend_purchase_order.Services;

public class AuthService(AppDbContext context, IConfiguration configuration)
{
    private readonly AppDbContext _context = context;
    private readonly IConfiguration _configuration = configuration;

    public async Task<string?> LoginAsync(LoginDto loginDto)
    {
        var user = await _context.User.SingleOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user == null || !user.ValidatePassword(loginDto.Password))
        {
            return null;
        }

        return GenerateJwtToken(user);
    }

    public async Task<User> RegisterAsync(UserDto userDto)
    {
        if (await _context.User.AnyAsync(u => u.Email == userDto.Email))
        {
            throw new ArgumentException("Email already exists");
        }

        var user = new User(userDto.Email, userDto.Password, userDto.Role);
        _context.User.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    private string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "a_super_secret_key_that_is_at_least_32_bytes_long_1234567890"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "backend_purchase_order",
            audience: _configuration["Jwt:Audience"] ?? "backend_purchase_order",
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}