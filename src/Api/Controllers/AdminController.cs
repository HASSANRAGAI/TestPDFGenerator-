using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestPDFGenerator.Api.Data;
using TestPDFGenerator.Api.Models;
using TestPDFGenerator.Api.Services;

namespace TestPDFGenerator.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ICustomJoinValidator _validator;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        ApplicationDbContext db,
        ICustomJoinValidator validator,
        ILogger<AdminController> logger)
    {
        _db = db;
        _validator = validator;
        _logger = logger;
    }

    /// <summary>
    /// Get list of all entities in the schema
    /// </summary>
    [HttpGet("schema/entities")]
    public ActionResult<List<string>> GetEntities()
    {
        try
        {
            var model = _db.Model;
            var entities = model.GetEntityTypes()
                .Select(e => e.ClrType.Name)
                .Where(name => !string.IsNullOrEmpty(name))
                .OrderBy(name => name)
                .ToList();

            return Ok(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entities");
            return StatusCode(500, new { error = "An error occurred while retrieving entities" });
        }
    }

    /// <summary>
    /// Get fields for a specific entity
    /// </summary>
    [HttpGet("schema/entities/{entityName}/fields")]
    public ActionResult<List<string>> GetEntityFields(string entityName)
    {
        try
        {
            var model = _db.Model;
            var entityType = model.FindEntityType($"TestPDFGenerator.Api.Models.{entityName}");
            
            if (entityType == null)
                return NotFound(new { error = $"Entity '{entityName}' not found" });

            var fields = entityType.GetProperties()
                .Select(p => p.Name)
                .OrderBy(name => name)
                .ToList();

            return Ok(fields);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity fields for {EntityName}", entityName);
            return StatusCode(500, new { error = "An error occurred while retrieving entity fields" });
        }
    }

    /// <summary>
    /// Validate custom joins configuration
    /// </summary>
    [HttpPost("validate-joins")]
    public async Task<ActionResult<ValidationResult>> ValidateJoins(
        [FromBody] ValidateJoinsRequest req,
        CancellationToken ct)
    {
        try
        {
            var schema = new Dictionary<string, object>();
            // In a real implementation, we would get the actual schema
            // For now, we validate basic structure
            
            var result = await _validator.ValidateAllAsync(req.Joins, schema);
            
            return Ok(new { errors = result.Errors, isValid = result.IsValid });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating joins");
            return StatusCode(500, new { error = "An error occurred while validating joins" });
        }
    }

    /// <summary>
    /// Refresh schema cache
    /// </summary>
    [HttpPost("refresh-schema")]
    public IActionResult RefreshSchema()
    {
        try
        {
            // Clear any schema caches if needed
            _logger.LogInformation("Schema cache refresh requested");
            return Ok(new { message = "Schema cache refreshed" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing schema");
            return StatusCode(500, new { error = "An error occurred while refreshing schema" });
        }
    }
}

public record ValidateJoinsRequest(string ContextName, List<CustomJoin> Joins);
