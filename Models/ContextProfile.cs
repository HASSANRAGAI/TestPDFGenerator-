namespace PdfTemplateSystem.Models;

public class ContextProfile
{
    public Guid Id { get; set; }
    public string ContextName { get; set; } = string.Empty;
    public string RootEntity { get; set; } = string.Empty;
    public List<string> IncludePaths { get; set; } = new();
    public List<string> AllowedFields { get; set; } = new();
    public Dictionary<string, string> Labels { get; set; } = new();
}
