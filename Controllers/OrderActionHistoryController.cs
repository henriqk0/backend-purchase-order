using backend_purchase_order.Models;
using backend_purchase_order.Models.DTOs;
using backend_purchase_order.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_purchase_order.Controllers;

/// <summary>
/// Classe para controle dos endpoints relativos ao modelo OrderActionHistory
/// </summary>
/// 
[ApiController]
[Route("api/[controller]")]
public class OrderActionHistoryController(
    OrderActionHistoryService orderActionHistoryService) : ControllerBase
{
    private readonly OrderActionHistoryService _orderActionHistoryService = orderActionHistoryService;

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderActionHistory>> Get(int id)
    {
        var orderHistory = await _orderActionHistoryService.GetOrderHistoryByIdAsync(id);

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