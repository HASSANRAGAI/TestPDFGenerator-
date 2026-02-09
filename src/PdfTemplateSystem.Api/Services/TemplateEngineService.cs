using HandlebarsDotNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PdfTemplateSystem.Data;
using PdfTemplateSystem.Models;
using System.Dynamic;

namespace PdfTemplateSystem.Services;

public class TemplateEngineService
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly IHandlebars _handlebars;
    private readonly ILogger<TemplateEngineService> _logger;
    private IHybridContextDataFetcher? _dataFetcher;
    private const string CacheKeyPrefix = "Template_";

    public TemplateEngineService(
        ApplicationDbContext context, 
        IMemoryCache cache,
        ILogger<TemplateEngineService> logger)
    {
        _context = context;
        _cache = cache;
        _handlebars = Handlebars.Create();
        _logger = logger;
    }

    // Allow setting the data fetcher to avoid circular dependency
    public void SetDataFetcher(IHybridContextDataFetcher dataFetcher)
    {
        _dataFetcher = dataFetcher;
    }

    public async Task<string> RenderTemplateAsync(Guid templateId, object data)
    {
        var template = await GetTemplateAsync(templateId);
        var compiledTemplate = GetCompiledTemplate(templateId, template.HtmlContent);
        
        return compiledTemplate(data);
    }

    public async Task<string> RenderTemplateByContextAsync(string contextName, Guid entityId, string? templateName = null)
    {
        // Get context profile
        var profile = await _context.ContextProfiles
            .FirstOrDefaultAsync(cp => cp.ContextName == contextName)
            ?? throw new InvalidOperationException($"Context profile '{contextName}' not found");

        // Get template
        var templateQuery = _context.PdfTemplates.Where(t => t.Context == contextName);
        if (!string.IsNullOrEmpty(templateName))
        {
            templateQuery = templateQuery.Where(t => t.Name == templateName);
        }
        
        var template = await templateQuery.FirstOrDefaultAsync()
            ?? throw new InvalidOperationException($"Template not found for context '{contextName}'");

        // Get entity data - use HybridContextDataFetcher if available and has custom joins
        object? shapedData;
        if (_dataFetcher != null && profile.CustomJoins.Any())
        {
            _logger.LogInformation("Using HybridContextDataFetcher for context: {Context}", contextName);
            shapedData = await _dataFetcher.GetSampleAsync(contextName, entityId, CancellationToken.None);
        }
        else
        {
            _logger.LogInformation("Using legacy data fetching for context: {Context}", contextName);
            shapedData = await GetShapedDataAsync(profile, entityId);
        }

        if (shapedData == null)
            throw new InvalidOperationException($"Entity with ID '{entityId}' not found");

        // Compile and render
        var compiledTemplate = GetCompiledTemplate(template.Id, template.HtmlContent);
        return compiledTemplate(shapedData);
    }

    private async Task<object> GetShapedDataAsync(ContextProfile profile, Guid entityId)
    {
        var cacheKey = $"{CacheKeyPrefix}ShapedData_{profile.ContextName}_{entityId}";
        
        if (_cache.TryGetValue(cacheKey, out object? cached) && cached != null)
        {
            return cached;
        }

        object? entity = profile.RootEntity switch
        {
            "SampleInvoice" => await GetSampleInvoiceAsync(entityId, profile),
            _ => throw new NotSupportedException($"Entity type '{profile.RootEntity}' not supported")
        };

        if (entity == null)
        {
            throw new InvalidOperationException($"Entity with ID '{entityId}' not found");
        }

        var shapedData = ShapeData(entity, profile, "");
        
        _cache.Set(cacheKey, shapedData, TimeSpan.FromMinutes(5));
        
        return shapedData;
    }

    private async Task<SampleInvoice?> GetSampleInvoiceAsync(Guid id, ContextProfile profile)
    {
        var query = _context.SampleInvoices.AsQueryable();

        // Apply includes based on profile
        foreach (var includePath in profile.IncludePaths)
        {
            query = query.Include(includePath);
        }

        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }

    private object ShapeData(object entity, ContextProfile profile, string pathPrefix)
    {
        if (entity == null) return new { };

        var entityType = entity.GetType();
        var result = new ExpandoObject() as IDictionary<string, object>;

        foreach (var property in entityType.GetProperties())
        {
            var fieldPath = string.IsNullOrEmpty(pathPrefix)
                ? property.Name
                : $"{pathPrefix}.{property.Name}";

            // Check if property is a navigation property
            if (property.PropertyType.Namespace != null && 
                property.PropertyType.Namespace.StartsWith("PdfTemplateSystem.Models"))
            {
                if (profile.IncludePaths.Any(ip => fieldPath.StartsWith(ip) || ip.StartsWith(fieldPath)))
                {
                    var navValue = property.GetValue(entity);
                    if (navValue != null)
                    {
                        result[property.Name] = ShapeData(navValue, profile, fieldPath);
                    }
                }
            }
            // Check if property is a collection
            else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType) && 
                     property.PropertyType != typeof(string))
            {
                if (profile.IncludePaths.Any(ip => fieldPath.StartsWith(ip) || ip.StartsWith(fieldPath)))
                {
                    var collectionValue = property.GetValue(entity) as System.Collections.IEnumerable;
                    if (collectionValue != null)
                    {
                        var shapedCollection = new List<object>();
                        foreach (var item in collectionValue)
                        {
                            shapedCollection.Add(ShapeData(item, profile, fieldPath));
                        }
                        result[property.Name] = shapedCollection;
                    }
                }
            }
            // Regular scalar properties
            else if (profile.AllowedFields.Contains(fieldPath))
            {
                var value = property.GetValue(entity);
                if (value != null)
                {
                    result[property.Name] = value;
                }
            }
        }

        return result;
    }

    private HandlebarsTemplate<object, object> GetCompiledTemplate(Guid templateId, string htmlContent)
    {
        var cacheKey = $"{CacheKeyPrefix}Compiled_{templateId}";
        
        if (_cache.TryGetValue(cacheKey, out HandlebarsTemplate<object, object>? cached) && cached != null)
        {
            return cached;
        }

        var compiled = _handlebars.Compile(htmlContent);
        
        _cache.Set(cacheKey, compiled, TimeSpan.FromHours(1));
        
        return compiled;
    }

    private async Task<PdfTemplate> GetTemplateAsync(Guid templateId)
    {
        return await _context.PdfTemplates.FindAsync(templateId)
            ?? throw new InvalidOperationException($"Template with ID '{templateId}' not found");
    }
}
