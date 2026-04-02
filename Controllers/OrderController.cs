using System.Security.Claims;
using backend_purchase_order.Models;
using backend_purchase_order.Models.DTOs;
using backend_purchase_order.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_purchase_order.Controllers;

/// <summary>
/// Classe para controle dos endpoints relativos aos pedidos
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrderController(OrderService orderService) : ControllerBase
{
    private readonly OrderService _orderService = orderService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> Get()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> Get(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);

        if (order == null)
            return NotFound();

        return order;
    }

    [HttpPost]
    public async Task<ActionResult<Order>> Post(OrderCreateDto orderDto)
    {
        try
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized();
            }

            var order = await _orderService.CreateOrderAsync(userId, orderDto);

            return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
        }
        catch (ArgumentException ex) // levantado pelo service
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, OrderUpdateDto orderDto)
    {
        try
        {
            await _orderService.UpdateOrderAsync(id, orderDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_orderService.OrderExists(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return NotFound();

        await _orderService.DeleteOrderAsync(id);

        return NoContent();
    }
}