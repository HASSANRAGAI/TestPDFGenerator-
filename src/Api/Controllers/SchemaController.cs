using Microsoft.AspNetCore.Mvc;
using TestPDFGenerator.Api.Services;

namespace TestPDFGenerator.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchemaController : ControllerBase
{
    private readonly SchemaDiscoveryService _schemaService;
    private readonly ILogger<SchemaController> _logger;

    public SchemaController(SchemaDiscoveryService schemaService, ILogger<SchemaController> logger)
    {
        _schemaService = schemaService;
        _logger = logger;
    }

    /// <summary>
    /// Get field tree for a specific context - shows only exposed fields per context profile
    /// </summary>
    [HttpGet("field-tree/{contextName}")]
    public async Task<ActionResult<Dictionary<string, object>>> GetFieldTree(string contextName)
    {
        try
        {
            var fieldTree = await _schemaService.GetFieldTreeForContext(contextName);
            return Ok(fieldTree);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Field tree not found for context: {ContextName}", contextName);
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting field tree for context: {ContextName}", contextName);
            return StatusCode(500, new { error = "An error occurred while retrieving the field tree" });
        }
    }

    /// <summary>
    /// Admin endpoint - get raw schema metadata for an entity type
    /// </summary>
    [HttpGet("metadata/{entityName}")]
    public ActionResult<Dictionary<string, object>> GetRawMetadata(string entityName)
    {
        try
        {
            var metadata = _schemaService.GetRawSchemaMetadata(entityName);
            return Ok(metadata);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Metadata not found for entity: {EntityName}", entityName);
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting metadata for entity: {EntityName}", entityName);
            return StatusCode(500, new { error = "An error occurred while retrieving the metadata" });
        }
    }
}
