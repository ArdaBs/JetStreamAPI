using JetStreamAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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

    /// <summary>
    /// Creates a new service order in the system.
    /// </summary>
    /// <remarks>
    /// This endpoint is responsible for creating a new service order. It requires 
    /// detailed information about the service order, which includes customer name, 
    /// contact details, service type, priority, and any additional comments.
    /// The creation and pickup dates are determined based on the provided priority.
    /// </remarks>
    /// <param name="serviceOrderDto">Data Transfer Object containing the service order details.</param>
    /// <returns>The ID of the newly created service order.</returns>
    /// <response code="200">Returns the ID of the newly created service order.</response>
    /// <response code="400">Returned if the provided data is invalid.</response>
    /// <response code="500">Returned if an error occurs while creating the service order.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateServiceOrder([FromBody] ServiceOrderDto serviceOrderDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

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

    /// <summary>
    /// Calculates the pickup date for a service order based on its priority.
    /// </summary>
    /// <param name="priority">The priority level of the service order.</param>
    /// <param name="creationDate">The date when the service order was created.</param>
    /// <returns>The calculated pickup date based on the priority.</returns>
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

    /// <summary>
    /// Retrieves all service orders.
    /// </summary>
    /// <remarks>
    /// This endpoint returns a list of all service orders available in the system.
    /// Requires authorization to access.
    /// </remarks>
    /// <returns>A list of service orders.</returns>
    /// <response code="200">Returns the list of all service orders.</response>
    /// <response code="400">Returned if the request is invalid.</response>
    /// <response code="500">Returned if an internal server error occurs.</response>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllServiceOrders()
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

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


    /// <summary>
    /// Searches for service orders by customer name.
    /// </summary>
    /// <remarks>
    /// This endpoint allows searching for service orders based on partial or full customer names.
    /// Requires authorization to access.
    /// </remarks>
    /// <param name="name">The name or partial name of the customer to search for.</param>
    /// <returns>A list of service orders matching the provided name.</returns>
    /// <response code="200">Returns the list of service orders matching the search criteria.</response>
    /// <response code="400">Returned if the request is invalid.</response>
    /// <response code="500">Returned if an internal server error occurs.</response>
    [HttpGet("search")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchServiceOrdersByName([FromQuery] string name)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

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

    /// <summary>
    /// Updates the comment of a specific service order.
    /// </summary>
    /// <remarks>
    /// This endpoint allows updating the comment field of a service order identified by its ID.
    /// Requires authorization to access.
    /// </remarks>
    /// <param name="id">The unique ID of the service order to be updated.</param>
    /// <param name="updateCommentDto">The data transfer object containing the new comment.</param>
    /// <returns>A status indicating the update was successful or not.</returns>
    /// <response code="204">Returned if the update is successful.</response>
    /// <response code="400">Returned if the request is invalid.</response>
    /// <response code="404">Returned if the service order is not found.</response>
    /// <response code="500">Returned if an internal server error occurs.</response>
    [HttpPatch("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateServiceOrderComment(int id, [FromBody] UpdateCommentDto updateCommentDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

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

    /// <summary>
    /// Deletes a specific service order.
    /// </summary>
    /// <remarks>
    /// This endpoint allows the deletion of a service order identified by its ID.
    /// Requires authorization to access.
    /// </remarks>
    /// <param name="id">The unique ID of the service order to be deleted.</param>
    /// <returns>A status indicating the deletion was successful or not.</returns>
    /// <response code="204">Returned if the deletion is successful.</response>
    /// <response code="400">Returned if the request is invalid.</response>
    /// <response code="404">Returned if the service order is not found.</response>
    /// <response code="500">Returned if an internal server error occurs.</response>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteServiceOrder(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

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

    /// <summary>
    /// Deletes all service orders in the system.
    /// </summary>
    /// <remarks>
    /// This endpoint allows the deletion of all service orders stored in the database.
    /// Requires authorization to access.
    /// </remarks>
    /// <returns>A status indicating if all service orders were successfully deleted.</returns>
    /// <response code="204">Returned if all service orders are successfully deleted.</response>
    /// <response code="400">Returned if the request is invalid.</response>
    /// <response code="500">Returned if an internal server error occurs.</response>
    [HttpDelete]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAllServiceOrders()
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

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

    /// <summary>
    /// Retrieves all service orders, optionally filtered by priority.
    /// </summary>
    /// <remarks>
    /// This endpoint fetches service orders from the database. 
    /// It can filter the orders based on the provided priority value.
    /// Requires authorization to access.
    /// </remarks>
    /// <param name="priority">The priority by which to filter service orders. Optional.</param>
    /// <returns>A list of service orders, possibly filtered by priority.</returns>
    /// <response code="200">Returns the list of service orders, filtered by priority if specified.</response>
    /// <response code="400">Returned if the request is invalid.</response>
    /// <response code="500">Returned if an internal server error occurs.</response>
    [HttpGet("ByPriority")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllServiceOrders([FromQuery] string priority)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

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

    /// <summary>
    /// Updates the status of a specific service order.
    /// </summary>
    /// <remarks>
    /// This endpoint allows updating the status of a service order identified by its ID. 
    /// The new status must be one of the predefined valid statuses: "Offen", "InBearbeitung", or "Abgeschlossen".
    /// Requires authorization to access.
    /// </remarks>
    /// <param name="id">The ID of the service order to update.</param>
    /// <param name="updateStatusDto">Object containing the new status.</param>
    /// <returns>No content on successful update, appropriate error message otherwise.</returns>
    /// <response code="204">Returned if the status is successfully updated.</response>
    /// <response code="400">Returned if the request is invalid or the provided status is not valid.</response>
    /// <response code="404">Returned if the service order with the specified ID is not found.</response>
    /// <response code="500">Returned if an internal server error occurs.</response>
    [HttpPatch("{id}/status")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto updateStatusDto)
    {
        // Validation
        var gültigeStatus = new List<string> { "Offen", "InBearbeitung", "Abgeschlossen" };

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Übergebene Daten ungültig.");
            return BadRequest(ModelState);
        }

        if (!gültigeStatus.Contains(updateStatusDto.Status))
        {
            _logger.LogWarning($"Ungültiger Status: {updateStatusDto.Status}.");
            return BadRequest($"Ungültiger Status: {updateStatusDto.Status}.");
        }

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
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [Phone]
    public string Phone { get; set; }

    [Required]
    public string Priority { get; set; }

    [Required]
    public string Service { get; set; }

    [Required]
    public DateTime CreationDate { get; set; }

    [Required]
    public DateTime PickupDate { get; set; }

    public string Comment { get; set; }
    public string Status { get; set; }
}
