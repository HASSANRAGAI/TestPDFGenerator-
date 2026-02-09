namespace PdfTemplateSystem.Models;

public class PdfTemplate
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public string HtmlContent { get; set; } = string.Empty;
    public Guid? DefaultSampleEntityId { get; set; }
}
