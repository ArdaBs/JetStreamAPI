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

    public EmployeesController(JetStreamBackendConfiguration context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto createEmployeeDto)
    {
        //check if username is already taken
        var existingUser = await _context.Employees
                                         .AnyAsync(e => e.Username == createEmployeeDto.Username);
        if (existingUser)
        {
            return BadRequest("Ein Benutzer mit diesem Benutzernamen existiert bereits.");
        }

        // if not taken then create user object
        var employee = new Employee
        {
            Username = createEmployeeDto.Username,
            Password = createEmployeeDto.Password,
        };

        // add to database
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Benutzer erfolgreich erstellt.", EmployeeId = employee.EmployeeId });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var employee = await _context.Employees
                                     .FirstOrDefaultAsync(e => e.Username == loginDto.Username);

        if (employee == null)
        {
            return Unauthorized("Benutzer nicht gefunden.");
        }

        if (employee.IsLocked)
        {
            return Unauthorized("Dieses Konto ist gesperrt.");
        }

        if (employee.Password != loginDto.Password)
        {
            employee.ProcessFailedLogin();

            await _context.SaveChangesAsync();

            if (employee.IsLocked)
            {
                // locked account
                return Unauthorized("Ihr Konto wurde aufgrund zu vieler fehlgeschlagener Login-Versuche gesperrt.");
            }
            else
            {
                // false input but not locked
                return Unauthorized("Ungültige Anmeldedaten.");
            }
        }

        // reset trys
        employee.ResetLock();
        await _context.SaveChangesAsync();

        // create JWT token
        var token = _tokenService.CreateToken(employee.Username);
        return Ok(new { userName = employee.Username, token = token });
    }

    [HttpPost("unlock/{username}")]
    [Authorize]
    public async Task<IActionResult> UnlockEmployee(string username)
    {
        var employeeToUnlock = await _context.Employees
                                             .FirstOrDefaultAsync(e => e.Username == username);

        if (employeeToUnlock == null)
        {
            return NotFound("Mitarbeiter nicht gefunden.");
        }

        // unlock employee
        employeeToUnlock.IsLocked = false;
        employeeToUnlock.FailedLoginAttempts = 0;

        // save changes in database
        await _context.SaveChangesAsync();

        return Ok($"Mitarbeiter {username} wurde erfolgreich entsperrt.");
    }

    [HttpGet("validate")]
    [Authorize]
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
