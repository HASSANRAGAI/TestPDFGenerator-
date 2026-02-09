using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using TestPDFGenerator.Api.Data;
using TestPDFGenerator.Api.Models;

namespace TestPDFGenerator.Api.Services;

public class SchemaDiscoveryService
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private const string CacheKeyPrefix = "Schema_";

    public SchemaDiscoveryService(ApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Dictionary<string, object>> GetFieldTreeForContext(string contextName)
    {
        var cacheKey = $"{CacheKeyPrefix}FieldTree_{contextName}";
        
        if (_cache.TryGetValue(cacheKey, out Dictionary<string, object>? cached) && cached != null)
        {
            return cached;
        }

        var profile = await _context.ContextProfiles
            .FirstOrDefaultAsync(cp => cp.ContextName == contextName);

        if (profile == null)
        {
            throw new InvalidOperationException($"Context profile '{contextName}' not found");
        }

        var fieldTree = BuildFieldTree(profile);
        
        _cache.Set(cacheKey, fieldTree, TimeSpan.FromHours(1));
        
        return fieldTree;
    }

    public Dictionary<string, object> GetRawSchemaMetadata(string entityName)
    {
        var cacheKey = $"{CacheKeyPrefix}RawMetadata_{entityName}";
        
        if (_cache.TryGetValue(cacheKey, out Dictionary<string, object>? cached) && cached != null)
        {
            return cached;
        }

        var model = _context.Model;
        var entityType = model.FindEntityType($"TestPDFGenerator.Api.Models.{entityName}");

        if (entityType == null)
        {
            throw new InvalidOperationException($"Entity type '{entityName}' not found in model");
        }

        var metadata = new Dictionary<string, object>
        {
            ["EntityName"] = entityType.Name,
            ["TableName"] = entityType.ClrType.Name,
            ["Properties"] = GetProperties(entityType),
            ["Navigations"] = GetNavigations(entityType)
        };

        _cache.Set(cacheKey, metadata, TimeSpan.FromHours(1));

        return metadata;
    }

    private Dictionary<string, object> BuildFieldTree(ContextProfile profile)
    {
        var model = _context.Model;
        var entityType = model.FindEntityType($"TestPDFGenerator.Api.Models.{profile.RootEntity}");

        if (entityType == null)
        {
            throw new InvalidOperationException($"Entity type '{profile.RootEntity}' not found");
        }

        var fieldTree = new Dictionary<string, object>
        {
            ["Context"] = profile.ContextName,
            ["RootEntity"] = profile.RootEntity,
            ["Fields"] = BuildFieldsRecursive(entityType, profile, "", 0)
        };

        return fieldTree;
    }

    private List<Dictionary<string, object>> BuildFieldsRecursive(
        IEntityType entityType, 
        ContextProfile profile, 
        string pathPrefix, 
        int depth)
    {
        if (depth > 2) return new List<Dictionary<string, object>>();

        var fields = new List<Dictionary<string, object>>();

        // Add scalar properties
        foreach (var property in entityType.GetProperties())
        {
            var fieldPath = string.IsNullOrEmpty(pathPrefix) 
                ? property.Name 
                : $"{pathPrefix}.{property.Name}";

            if (profile.AllowedFields.Contains(fieldPath))
            {
                var label = profile.Labels.ContainsKey(fieldPath) 
                    ? profile.Labels[fieldPath] 
                    : property.Name;

                fields.Add(new Dictionary<string, object>
                {
                    ["Name"] = property.Name,
                    ["Path"] = fieldPath,
                    ["Label"] = label,
                    ["Type"] = property.ClrType.Name,
                    ["IsNullable"] = property.IsNullable
                });
            }
        }

        // Add navigations
        foreach (var navigation in entityType.GetNavigations())
        {
            var navPath = string.IsNullOrEmpty(pathPrefix)
                ? navigation.Name
                : $"{pathPrefix}.{navigation.Name}";

            if (profile.IncludePaths.Any(ip => navPath.StartsWith(ip) || ip.StartsWith(navPath)))
            {
                var targetType = navigation.TargetEntityType;
                var label = profile.Labels.ContainsKey(navPath)
                    ? profile.Labels[navPath]
                    : navigation.Name;

                var navFields = BuildFieldsRecursive(targetType, profile, navPath, depth + 1);

                fields.Add(new Dictionary<string, object>
                {
                    ["Name"] = navigation.Name,
                    ["Path"] = navPath,
                    ["Label"] = label,
                    ["Type"] = navigation.IsCollection ? "Collection" : "Navigation",
                    ["IsCollection"] = navigation.IsCollection,
                    ["TargetType"] = targetType.ClrType.Name,
                    ["Fields"] = navFields
                });
            }
        }

        return fields;
    }

    private List<Dictionary<string, string>> GetProperties(IEntityType entityType)
    {
        return entityType.GetProperties()
            .Select(p => new Dictionary<string, string>
            {
                ["Name"] = p.Name,
                ["Type"] = p.ClrType.Name,
                ["IsNullable"] = p.IsNullable.ToString(),
                ["IsPrimaryKey"] = p.IsPrimaryKey().ToString()
            })
            .ToList();
    }

    private List<Dictionary<string, string>> GetNavigations(IEntityType entityType)
    {
        return entityType.GetNavigations()
            .Select(n => new Dictionary<string, string>
            {
                ["Name"] = n.Name,
                ["TargetType"] = n.TargetEntityType.ClrType.Name,
                ["IsCollection"] = n.IsCollection.ToString(),
                ["ForeignKey"] = n.ForeignKey.Properties.FirstOrDefault()?.Name ?? ""
            })
            .ToList();
    }
}
