using backend_purchase_order.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend_purchase_order.Controllers;

/// <summary>
/// Classe para controle dos endpoints relativos aos pedidos
/// </summary>
[Route("api/[controller]")]
public class OrderController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> Get(int id)
    {
        var order = await _context.Order.FindAsync(id);

        if (order == null)
            return NotFound();

        return order;
    }

    [HttpPost]
    public async Task<ActionResult<Order>> Post(Order order)
    {
        _context.Order.Add(order);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
    }
}