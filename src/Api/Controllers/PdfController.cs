using Microsoft.AspNetCore.Mvc;
using TestPDFGenerator.Api.Services;

namespace TestPDFGenerator.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PdfController : ControllerBase
{
    private readonly TemplateEngineService _templateEngine;
    private readonly PdfGenerationService _pdfGenerator;
    private readonly ILogger<PdfController> _logger;

    public PdfController(
        TemplateEngineService templateEngine,
        PdfGenerationService pdfGenerator,
        ILogger<PdfController> logger)
    {
        _templateEngine = templateEngine;
        _pdfGenerator = pdfGenerator;
        _logger = logger;
    }

    /// <summary>
    /// Generate PDF for a specific context and entity
    /// </summary>
    [HttpGet("generate/{contextName}/{entityId}")]
    public async Task<IActionResult> GeneratePdf(string contextName, Guid entityId, [FromQuery] string? templateName = null)
    {
        try
        {
            _logger.LogInformation("Generating PDF for context: {Context}, entity: {EntityId}", contextName, entityId);

            // Render HTML from template
            var html = await _templateEngine.RenderTemplateByContextAsync(contextName, entityId, templateName);

            // Generate PDF
            var pdfBytes = await _pdfGenerator.GeneratePdfFromHtmlAsync(html);

            _logger.LogInformation("PDF generated successfully, size: {Size} bytes", pdfBytes.Length);

            // Return PDF file
            return File(pdfBytes, "application/pdf", $"{contextName}_{entityId}.pdf");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "PDF generation failed");
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating PDF");
            return StatusCode(500, new { error = "An error occurred while generating the PDF" });
        }
    }

    /// <summary>
    /// Preview HTML for a specific context and entity
    /// </summary>
    [HttpGet("preview/{contextName}/{entityId}")]
    public async Task<IActionResult> PreviewHtml(string contextName, Guid entityId, [FromQuery] string? templateName = null)
    {
        try
        {
            _logger.LogInformation("Previewing HTML for context: {Context}, entity: {EntityId}", contextName, entityId);

            var html = await _templateEngine.RenderTemplateByContextAsync(contextName, entityId, templateName);

            return Content(html, "text/html");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "HTML preview failed");
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error previewing HTML");
            return StatusCode(500, new { error = "An error occurred while previewing the HTML" });
        }
    }

    /// <summary>
    /// Generate PDF from custom HTML
    /// </summary>
    [HttpPost("generate-from-html")]
    public async Task<IActionResult> GenerateFromHtml([FromBody] GenerateFromHtmlRequest request)
    {
        try
        {
            _logger.LogInformation("Generating PDF from custom HTML");

            var pdfBytes = await _pdfGenerator.GeneratePdfFromHtmlAsync(request.Html);

            _logger.LogInformation("PDF generated successfully, size: {Size} bytes", pdfBytes.Length);

            return File(pdfBytes, "application/pdf", "custom.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating PDF from HTML");
            return StatusCode(500, new { error = "An error occurred while generating the PDF" });
        }
    }
}

public class GenerateFromHtmlRequest
{
    public string Html { get; set; } = string.Empty;
}
