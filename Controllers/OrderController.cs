using backend_purchase_order.Models;
using backend_purchase_order.Models.DTOs;
using backend_purchase_order.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_purchase_order.Controllers;

/// <summary>
/// Classe para controle dos endpoints relativos aos pedidos
/// </summary>
[Route("api/[controller]")]
public class OrderController(AppDbContext context, OrderService orderService) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly OrderService _orderService = orderService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> Get()
    {
        return await _context.Order.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> Get(int id)
    {
        var order = await _context.Order.FindAsync(id);

        if (order == null)
            return NotFound();

        return order;
    }

    [HttpPost]
    public async Task<ActionResult<Order>> Post(OrderCreateDto orderDto)
    {
        try
        {
            var user = await _context.User.FindAsync(orderDto.OrderMakerId);

            if (user == null)
                return BadRequest("User not found");

            var order = await _orderService.CreateOrderAsync(orderDto);

            return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Order order)
    {
        if (id != order.Id)
            return BadRequest();

        _context.Entry(order).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Order.Any(e => e.Id == id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var order = await _context.Order.FindAsync(id);
        if (order == null)
            return NotFound();

        _context.Order.Remove(order);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}