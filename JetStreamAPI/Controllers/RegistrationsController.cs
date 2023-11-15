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
        // Create a new ServiceOrder object from the DTO
        var serviceOrder = new ServiceOrder
        {
            CustomerName = serviceOrderDto.Name,
            Email = serviceOrderDto.Email,
            PhoneNumber = serviceOrderDto.Phone,
            Priority = serviceOrderDto.Priority,
            ServiceType = serviceOrderDto.Service,
            CreationDate = serviceOrderDto.CreateDate,
            PickupDate = serviceOrderDto.PickupDate,
            Comments = serviceOrderDto.Comment
        };

        _context.ServiceOrders.Add(serviceOrder);
        await _context.SaveChangesAsync();

        // Return a confirmation with the ID of the created service order
        return Ok(new { id = serviceOrder.Id });
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
}
