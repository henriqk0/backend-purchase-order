using backend_purchase_order.Models;
using backend_purchase_order.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_purchase_order.Controllers;

/// <summary>
/// Classe para controle dos endpoints relativos aos usuarios
/// TODO: lidar com Autorizacoeses e Autenticacoes, p.ex. com JWT, mais complexas
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> Get()
    {
        return await _context.User.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> Get(int id)
    {
        var user = await _context.User.FindAsync(id);

        if (user == null)
            return NotFound();

        return user;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, UserDto userDto)
    {
        var user = await _context.User.FindAsync(id);

        if (user == null)
            return NotFound();

        user.SetEmail(userDto.Email);
        user.SetPassword(userDto.Password);
        user.SetRole(userDto.Role);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.User.Any(e => e.Id == id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _context.User.FindAsync(id);
        if (user == null)
            return NotFound();

        _context.User.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}