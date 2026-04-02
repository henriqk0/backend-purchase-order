using backend_purchase_order.Models;
using backend_purchase_order.Models.DTOs;
using backend_purchase_order.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend_purchase_order.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService authService) : ControllerBase
{
    private readonly AuthService _authService = authService;

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var token = await _authService.LoginAsync(loginDto);

        if (token == null)
        {
            return Unauthorized("Invalid credentials.");
        }

        return Ok(new { Token = token });
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(UserDto userDto)
    {
        try
        {
            var user = await _authService.RegisterAsync(userDto);
            return Ok(user);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}