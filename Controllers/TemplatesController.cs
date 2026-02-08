using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfTemplateSystem.Data;
using PdfTemplateSystem.Models;

namespace PdfTemplateSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TemplatesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TemplatesController> _logger;

    public TemplatesController(ApplicationDbContext context, ILogger<TemplatesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all templates
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PdfTemplate>>> GetTemplates()
    {
        return await _context.PdfTemplates.ToListAsync();
    }

    /// <summary>
    /// Get template by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PdfTemplate>> GetTemplate(Guid id)
    {
        var template = await _context.PdfTemplates.FindAsync(id);

        if (template == null)
        {
            return NotFound();
        }

        return template;
    }

    /// <summary>
    /// Get templates by context
    /// </summary>
    [HttpGet("by-context/{contextName}")]
    public async Task<ActionResult<IEnumerable<PdfTemplate>>> GetTemplatesByContext(string contextName)
    {
        return await _context.PdfTemplates
            .Where(t => t.Context == contextName)
            .ToListAsync();
    }

    /// <summary>
    /// Create new template
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PdfTemplate>> CreateTemplate(PdfTemplate template)
    {
        if (template.Id == Guid.Empty)
        {
            template.Id = Guid.NewGuid();
        }

        _context.PdfTemplates.Add(template);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTemplate), new { id = template.Id }, template);
    }

    /// <summary>
    /// Update template
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTemplate(Guid id, PdfTemplate template)
    {
        if (id != template.Id)
        {
            return BadRequest();
        }

        _context.Entry(template).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await TemplateExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Delete template
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTemplate(Guid id)
    {
        var template = await _context.PdfTemplates.FindAsync(id);
        if (template == null)
        {
            return NotFound();
        }

        _context.PdfTemplates.Remove(template);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> TemplateExists(Guid id)
    {
        return await _context.PdfTemplates.AnyAsync(e => e.Id == id);
    }
}
