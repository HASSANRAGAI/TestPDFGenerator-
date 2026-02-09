using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestPDFGenerator.Api.Data;
using TestPDFGenerator.Api.Models;

namespace TestPDFGenerator.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContextProfilesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ContextProfilesController> _logger;

    public ContextProfilesController(ApplicationDbContext context, ILogger<ContextProfilesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all context profiles
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ContextProfile>>> GetContextProfiles()
    {
        return await _context.ContextProfiles.ToListAsync();
    }

    /// <summary>
    /// Get context profile by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ContextProfile>> GetContextProfile(Guid id)
    {
        var profile = await _context.ContextProfiles.FindAsync(id);

        if (profile == null)
        {
            return NotFound();
        }

        return profile;
    }

    /// <summary>
    /// Get context profile by name
    /// </summary>
    [HttpGet("by-name/{contextName}")]
    public async Task<ActionResult<ContextProfile>> GetContextProfileByName(string contextName)
    {
        var profile = await _context.ContextProfiles
            .FirstOrDefaultAsync(cp => cp.ContextName == contextName);

        if (profile == null)
        {
            return NotFound();
        }

        return profile;
    }

    /// <summary>
    /// Create new context profile
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ContextProfile>> CreateContextProfile(ContextProfile profile)
    {
        if (profile.Id == Guid.Empty)
        {
            profile.Id = Guid.NewGuid();
        }

        _context.ContextProfiles.Add(profile);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetContextProfile), new { id = profile.Id }, profile);
    }

    /// <summary>
    /// Update context profile
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateContextProfile(Guid id, ContextProfile profile)
    {
        if (id != profile.Id)
        {
            return BadRequest();
        }

        _context.Entry(profile).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ProfileExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Delete context profile
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContextProfile(Guid id)
    {
        var profile = await _context.ContextProfiles.FindAsync(id);
        if (profile == null)
        {
            return NotFound();
        }

        _context.ContextProfiles.Remove(profile);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> ProfileExists(Guid id)
    {
        return await _context.ContextProfiles.AnyAsync(e => e.Id == id);
    }
}
