using backend_purchase_order.Models;
using backend_purchase_order.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_purchase_order.Controllers;

/// <summary>
/// Classe para controle dos endpoints relativos aos itens
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ItemController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Item>>> Get()
    {
        return await _context.Item.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Item>> Get(int id)
    {
        var item = await _context.Item.FindAsync(id);

        if (item == null)
            return NotFound();

        return item;
    }

    [HttpPost]
    public async Task<ActionResult<Item>> Post(ItemDto itemDto)

    {
        var item = new Item((int)itemDto.Value, itemDto.Name);

        _context.Item.Add(item);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, ItemDto itemDto)
    {
        var item = new Item((int)itemDto.Value, itemDto.Name);

        if (id != item.Id)
            return BadRequest();

        _context.Entry(item).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Item.Any(e => e.Id == id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Item.FindAsync(id);
        if (item == null)
            return NotFound();

        _context.Item.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}