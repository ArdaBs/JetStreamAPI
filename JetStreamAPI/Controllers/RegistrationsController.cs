using JetStreamAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class RegistrationsController : ControllerBase
{
    private readonly JetStreamBackendConfiguration _context;
    private readonly ILogger<EmployeesController> _logger;

    public RegistrationsController(JetStreamBackendConfiguration context, ILogger<EmployeesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateServiceOrder([FromBody] ServiceOrderDto serviceOrderDto)
    {
        _logger.LogInformation("Versuch, einen neuen Serviceauftrag zu erstellen.");

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

            _logger.LogInformation($"Serviceauftrag erfolgreich erstellt. ID: {serviceOrder.Id}");
            return Ok(new { id = serviceOrder.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Erstellen eines Serviceauftrags.");
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
        _logger.LogInformation("Anfrage zum Abrufen aller Serviceaufträge erhalten.");

        try
        {
            var serviceOrders = await _context.ServiceOrders.ToListAsync();
            _logger.LogInformation($"Abfrage erfolgreich. {serviceOrders.Count} Serviceaufträge abgerufen.");
            return Ok(serviceOrders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Abrufen der Serviceaufträge.");
            return StatusCode(500, "Ein Fehler ist beim Abrufen der Serviceaufträge aufgetreten.");
        }
    }


    // GET: api/serviceorders/search?name=JohnDoe
    [HttpGet("search")]
    [Authorize]
    public async Task<IActionResult> SearchServiceOrdersByName([FromQuery] string name)
    {
        try
        {
            var serviceOrders = await _context.ServiceOrders
                                              .Where(so => so.CustomerName.Contains(name))
                                              .ToListAsync();
            _logger.LogInformation($"Gefunden {serviceOrders.Count} Serviceaufträge mit Namen: {name}");
            return Ok(serviceOrders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Fehler bei der Suche nach Serviceaufträgen mit Namen: {name}");
            return StatusCode(500, "Ein Fehler ist aufgetreten beim Abrufen der Serviceaufträge.");
        }
    }

    // PATCH: api/registrations/5
    [HttpPatch("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateServiceOrderComment(int id, [FromBody] UpdateCommentDto updateCommentDto)
    {
        try
        {
            var serviceOrder = await _context.ServiceOrders.FindAsync(id);
            if (serviceOrder == null)
            {
                _logger.LogWarning($"Serviceauftrag mit ID {id} nicht gefunden.");
                return NotFound();
            }

            serviceOrder.Comments = updateCommentDto.Comment;
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Kommentar für Serviceauftrag ID {id} aktualisiert.");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Fehler bei der Aktualisierung des Kommentars für Serviceauftrag ID {id}.");
            return StatusCode(500, "Ein Fehler ist aufgetreten beim Aktualisieren des Serviceauftrags.");
        }
    }

    // DELETE: api/registrations/5
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteServiceOrder(int id)
    {
        try
        {
            var serviceOrder = await _context.ServiceOrders.FindAsync(id);
            if (serviceOrder == null)
            {
                _logger.LogWarning($"Serviceauftrag mit ID {id} nicht gefunden.");
                return NotFound();
            }

            _context.ServiceOrders.Remove(serviceOrder);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Serviceauftrag mit ID {id} wurde gelöscht.");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Fehler beim Löschen des Serviceauftrags mit ID {id}.");
            return StatusCode(500, "Ein Fehler ist aufgetreten beim Löschen des Serviceauftrags.");
        }
    }

    // DELETE: api/registrations
    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteAllServiceOrders()
    {
        try
        {
            _context.ServiceOrders.RemoveRange(_context.ServiceOrders);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Alle Serviceaufträge wurden gelöscht.");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Löschen aller Serviceaufträge.");
            return StatusCode(500, "Ein Fehler ist aufgetreten beim Löschen aller Serviceaufträge.");
        }
    }

    // GET: api/serviceorders
    [HttpGet("ByPriority")]
    [Authorize]
    public async Task<IActionResult> GetAllServiceOrders([FromQuery] string priority)
    {
        try
        {
            IQueryable<ServiceOrder> query = _context.ServiceOrders;

            if (!string.IsNullOrEmpty(priority))
            {
                // Filter Service Orders based on priority
                query = query.Where(so => so.Priority.ToLower() == priority.ToLower());
                _logger.LogInformation($"Serviceaufträge werden nach Priorität gefiltert: {priority}.");
            }
            else
            {
                _logger.LogInformation("Alle Serviceaufträge werden abgerufen.");
            }

            var serviceOrders = await query.ToListAsync();
            return Ok(serviceOrders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Abrufen von Serviceaufträgen.");
            return StatusCode(500, "Ein Fehler ist aufgetreten beim Abrufen von Serviceaufträgen.");
        }
    }

    [HttpPatch("{id}/status")]
    [Authorize]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto updateStatusDto)
    {
        try
        {
            var order = await _context.ServiceOrders.FindAsync(id);
            if (order == null)
            {
                _logger.LogWarning($"Serviceauftrag mit ID {id} nicht gefunden.");
                return NotFound();
            }

            _logger.LogInformation($"Aktualisiere den Status des Serviceauftrags mit ID {id} auf {updateStatusDto.Status}.");
            order.Status = updateStatusDto.Status;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Serviceauftrag mit ID {id} erfolgreich auf {updateStatusDto.Status} aktualisiert.");
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Fehler beim Aktualisieren des Status für Serviceauftrag mit ID {id}.");
            return StatusCode(500, "Ein Fehler ist aufgetreten beim Aktualisieren des Serviceauftrags.");
        }
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
