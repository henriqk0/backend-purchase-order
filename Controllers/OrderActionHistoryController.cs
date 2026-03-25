using backend_purchase_order.Models;
using backend_purchase_order.Models.DTOs;
using backend_purchase_order.Models.Enums;
using backend_purchase_order.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_purchase_order.Controllers;

/// <summary>
/// Classe para controle dos endpoints relativos ao modelo OrderActionHistory
/// </summary>
[Route("api/[controller]")]
public class OrderActionHistoryController(
    AppDbContext context,
    OrderActionHistoryService orderActionHistoryService) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly OrderActionHistoryService _orderActionHistoryService = orderActionHistoryService;

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderActionHistory>> Get(int id)
    {
        var orderHistory = await _context.OrderActionHistory.FindAsync(id);

        if (orderHistory == null)
            return NotFound();

        return orderHistory;
    }

    [HttpPost]
    public async Task<ActionResult<OrderActionHistory>> Post(RecordActionDto orderActionDto)
    {
        try
        {
            var orderActionHistory = await _orderActionHistoryService.RecordAgnosticAction(orderActionDto);

            return CreatedAtAction(nameof(Get), new { id = orderActionHistory.Id }, orderActionHistory);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

}