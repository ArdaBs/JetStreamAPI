using JetStreamAPI.Models;
using JetStreamAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly JetStreamBackendConfiguration _context;
    private readonly ITokenService _tokenService;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(JetStreamBackendConfiguration context, ITokenService tokenService, ILogger<EmployeesController> logger)
    {
        _context = context;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new employee in the system.
    /// </summary>
    /// <param name="createEmployeeDto">Data for creating a new employee.</param>
    /// <returns>Details of the created employee.</returns>
    /// <response code="200">Returns the details of the newly created employee.</response>
    /// <response code="400">Returned if the request is invalid or the username is already taken.</response>
    /// <response code="500">Returned if an internal server error occurs.</response>
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto createEmployeeDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _logger.LogInformation($"Versuch, einen neuen Benutzer zu erstellen: {createEmployeeDto.Username}");

        // check if user already exists
        var existingUser = await _context.Employees
                                         .AnyAsync(e => e.Username == createEmployeeDto.Username);
        if (existingUser)
        {
            _logger.LogWarning($"Erstellen des Benutzers fehlgeschlagen: Benutzername {createEmployeeDto.Username} ist bereits vergeben.");
            return BadRequest("Ein Benutzer mit diesem Benutzernamen existiert bereits.");
        }

        try
        {
            // creating user object
            var employee = new Employee
            {
                Username = createEmployeeDto.Username,
                Password = createEmployeeDto.Password,
            };

            // add to database
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Benutzer {createEmployeeDto.Username} erfolgreich erstellt. ID: {employee.EmployeeId}");
            return Ok(new { Message = "Benutzer erfolgreich erstellt.", EmployeeId = employee.EmployeeId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Fehler beim Erstellen des Benutzers {createEmployeeDto.Username}");
            return StatusCode(500, "Ein interner Fehler ist aufgetreten.");
        }
    }


    /// <summary>
    /// Allows a user to log in to the system.
    /// </summary>
    /// <param name="loginDto">Login credentials of the user.</param>
    /// <returns>A token on successful login.</returns>
    /// <response code="200">Returns a token if the login is successful.</response>
    /// <response code="400">Returned if the request is invalid.</response>
    /// <response code="401">Returned if the login credentials are invalid or the account is locked.</response>
    /// <response code="500">Returned if an internal server error occurs.</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _logger.LogInformation($"Loginversuch für Benutzer {loginDto.Username}");

        var employee = await _context.Employees
                                     .FirstOrDefaultAsync(e => e.Username == loginDto.Username);

        if (employee == null)
        {
            _logger.LogWarning($"Loginversuch fehlgeschlagen: Benutzer {loginDto.Username} nicht gefunden.");
            return Unauthorized("Benutzer nicht gefunden.");
        }

        if (employee.IsLocked)
        {
            _logger.LogWarning($"Loginversuch für gesperrten Benutzer {loginDto.Username}.");
            return Unauthorized("Dieses Konto ist gesperrt.");
        }

        if (employee.Password != loginDto.Password)
        {
            employee.ProcessFailedLogin();
            await _context.SaveChangesAsync();

            if (employee.IsLocked)
            {
                _logger.LogWarning($"Konto von {loginDto.Username} aufgrund zu vieler fehlgeschlagener Login-Versuche gesperrt.");
                return Unauthorized("Ihr Konto wurde aufgrund zu vieler fehlgeschlagener Login-Versuche gesperrt.");
            }
            else
            {
                _logger.LogWarning($"Fehlgeschlagener Loginversuch für Benutzer {loginDto.Username}: Ungültige Anmeldedaten.");
                return Unauthorized("Ungültige Anmeldedaten.");
            }
        }

        // Reset trys
        employee.ResetLock();
        await _context.SaveChangesAsync();

        // create JWT-Token
        var token = _tokenService.CreateToken(employee.Username);

        _logger.LogInformation($"Benutzer {loginDto.Username} erfolgreich eingeloggt.");
        return Ok(new { userName = employee.Username, token = token });
    }

    [HttpPost("unlock/{username}")]
    [Authorize]
    public async Task<IActionResult> UnlockEmployee(string username)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _logger.LogInformation($"Versuch, Benutzer {username} zu entsperren.");

        var employeeToUnlock = await _context.Employees
                                             .FirstOrDefaultAsync(e => e.Username == username);

        if (employeeToUnlock == null)
        {
            _logger.LogWarning($"Entsperrungsversuch fehlgeschlagen: Mitarbeiter {username} nicht gefunden.");
            return NotFound("Mitarbeiter nicht gefunden.");
        }

        if (!employeeToUnlock.IsLocked)
        {
            _logger.LogInformation($"Benutzer {username} ist bereits entsperrt.");
            return BadRequest($"Benutzer {username} ist bereits entsperrt.");
        }

        // unlock employee
        employeeToUnlock.IsLocked = false;
        employeeToUnlock.FailedLoginAttempts = 0;

        await _context.SaveChangesAsync();

        _logger.LogInformation($"Mitarbeiter {username} wurde erfolgreich entsperrt.");
        return Ok($"Mitarbeiter {username} wurde erfolgreich entsperrt.");
    }

    /// <summary>
    /// Validates the current user's token.
    /// </summary>
    /// <remarks>
    /// This API endpoint is used to verify if the current user's token is still valid.
    /// It's particularly useful for client applications to check the authentication 
    /// status of the user, ensuring that their session has not expired.
    /// </remarks>
    /// <returns>A confirmation message if the token is valid.</returns>
    /// <response code="200">Returns a success message if the token is valid.</response>
    /// <response code="401">Returned if the token is invalid, expired, or not provided.</response>
    [HttpGet("validate")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult ValidateToken()
    {
        return Ok(new { Message = "Token is valid." });
    }


    // DTO for login
    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    // DTO to create user
    public class CreateEmployeeDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
