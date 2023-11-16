using JetStreamAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class RegistrationsController : ControllerBase
{
    private readonly JetStreamBackendConfiguration _context;

    public RegistrationsController(JetStreamBackendConfiguration context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateServiceOrder([FromBody] ServiceOrderDto serviceOrderDto)
    {
        try
        {
            var creationDate = DateTime.UtcNow;
            var pickupDate = CalculatePickupDate(serviceOrderDto.Priority, creationDate);

            var serviceOrder = new ServiceOrder
            {
                CustomerName = serviceOrderDto.Name,
                Email = serviceOrderDto.Email,
                PhoneNumber = serviceOrderDto.Phone,
                Priority = serviceOrderDto.Priority,
                ServiceType = serviceOrderDto.Service,
                CreationDate = creationDate,
                PickupDate = pickupDate,
                Comments = serviceOrderDto.Comment,
                Status = serviceOrderDto.Status
            };

            _context.ServiceOrders.Add(serviceOrder);
            await _context.SaveChangesAsync();

            return Ok(new { id = serviceOrder.Id });
        }
        catch (Exception)
        {
            return StatusCode(500, "Ein Fehler ist beim Erstellen des Serviceauftrags aufgetreten");
        }
    }

    private DateTime CalculatePickupDate(string priority, DateTime creationDate)
    {
        return priority switch
        {
            "low" => creationDate.AddDays(12),
            "standard" => creationDate.AddDays(7),
            "express" => creationDate.AddDays(5),
            _ => creationDate
        };
    }


    // GET: api/serviceorders
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllServiceOrders()
    {
        var serviceOrders = await _context.ServiceOrders.ToListAsync();
        return Ok(serviceOrders);
    }

    // GET: api/serviceorders/search?name=JohnDoe
    [HttpGet("search")]
    [Authorize]
    public async Task<IActionResult> SearchServiceOrdersByName([FromQuery] string name)
    {
        var serviceOrders = await _context.ServiceOrders
                                          .Where(so => so.CustomerName.Contains(name))
                                          .ToListAsync();
        return Ok(serviceOrders);
    }

    // PATCH: api/registrations/5
    [HttpPatch("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateServiceOrderComment(int id, [FromBody] UpdateCommentDto updateCommentDto)
    {
        var serviceOrder = await _context.ServiceOrders.FindAsync(id);
        if (serviceOrder == null)
        {
            return NotFound();
        }

        serviceOrder.Comments = updateCommentDto.Comment;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/registrations/5
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteServiceOrder(int id)
    {
        var serviceOrder = await _context.ServiceOrders.FindAsync(id);
        if (serviceOrder == null)
        {
            return NotFound();
        }

        _context.ServiceOrders.Remove(serviceOrder);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/registrations
    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteAllServiceOrders()
    {
        _context.ServiceOrders.RemoveRange(_context.ServiceOrders);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/serviceorders
    [HttpGet("ByPriority")]
    [Authorize]
    public async Task<IActionResult> GetAllServiceOrders([FromQuery] string priority)
    {
        IQueryable<ServiceOrder> query = _context.ServiceOrders;

        if (!string.IsNullOrEmpty(priority))
        {
            // Filter Service Orders based on priority
            query = query.Where(so => so.Priority.ToLower() == priority.ToLower());
        }

        var serviceOrders = await query.ToListAsync();
        return Ok(serviceOrders);
    }

    [HttpPatch("{id}/status")]
    [Authorize]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto updateStatusDto)
    {
        var order = await _context.ServiceOrders.FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        order.Status = updateStatusDto.Status;
        await _context.SaveChangesAsync();

        return NoContent();
    }


}

// DTO for status
public class UpdateOrderStatusDto
{
    public string Status { get; set; }
}

// DTO for update of comments
public class UpdateCommentDto
{
    public string Comment { get; set; }
}

// DTO to get data
public class ServiceOrderDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Priority { get; set; }
    public string Service { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime PickupDate { get; set; }
    public string Comment { get; set; }
    public string Status { get; set; }
}
