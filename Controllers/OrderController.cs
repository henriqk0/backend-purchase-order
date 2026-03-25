using backend_purchase_order.Models;
using backend_purchase_order.Models.DTOs;
using backend_purchase_order.Models.Enums;
using backend_purchase_order.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_purchase_order.Controllers;

/// <summary>
/// Classe para controle dos endpoints relativos aos pedidos
/// </summary>
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
            var order = await _orderService.CreateOrderAsync(orderDto);

            return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
        }
        catch (ArgumentException ex) // levantado pelo service
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Order order)
    {
        if (id != order.Id)
            return BadRequest();

        try
        {
            await _orderService.UpdateOrderAsync(id, order);
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