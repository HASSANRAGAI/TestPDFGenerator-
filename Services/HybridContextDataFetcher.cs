using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using PdfTemplateSystem.Data;
using PdfTemplateSystem.Models;

namespace PdfTemplateSystem.Services;

public interface IHybridContextDataFetcher
{
    Task<object?> GetSampleAsync(string contextName, Guid id, CancellationToken ct);
}

public class HybridContextDataFetcher : IHybridContextDataFetcher
{
    private readonly ApplicationDbContext _db;
    private readonly TemplateEngineService _templateEngine;
    private readonly ILogger<HybridContextDataFetcher> _logger;

    public HybridContextDataFetcher(
        ApplicationDbContext db,
        TemplateEngineService templateEngine,
        ILogger<HybridContextDataFetcher> logger)
    {
        _db = db;
        _templateEngine = templateEngine;
        _logger = logger;
    }

    public async Task<object?> GetSampleAsync(string contextName, Guid id, CancellationToken ct)
    {
        var profile = await LoadProfileAsync(contextName, ct);

        if (profile.CustomJoins.Any())
        {
            _logger.LogInformation("Using custom joins for context: {Context}", contextName);
            // Use Dapper for custom joins
            return await FetchWithCustomJoins(profile, id, ct);
        }
        else
        {
            _logger.LogInformation("Using EF Core for context: {Context}", contextName);
            // Use existing EF Core implementation
            return await FetchWithEfCore(profile, id, ct);
        }
    }

    private async Task<ContextProfile> LoadProfileAsync(string contextName, CancellationToken ct)
    {
        var profile = await _db.ContextProfiles
            .FirstOrDefaultAsync(cp => cp.ContextName == contextName, ct);

        if (profile == null)
            throw new InvalidOperationException($"Context profile '{contextName}' not found");

        return profile;
    }

    private async Task<object?> FetchWithCustomJoins(ContextProfile profile, Guid id, CancellationToken ct)
    {
        // Build dynamic SQL query
        var sqlBuilder = new DynamicSqlQueryBuilder(_db);
        var (sql, parameters) = await sqlBuilder.BuildQueryAsync(profile, id);

        _logger.LogDebug("Generated SQL: {Sql}", sql);

        // Execute with Dapper
        using var connection = CreateConnection();
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }
        
        var results = await connection.QueryAsync(sql, parameters);

        if (!results.Any())
            return null;

        // Shape flat results to nested object structure
        var shaper = new ResultShaper(profile);
        return shaper.ShapeToNestedObject(results);
    }

    private async Task<object?> FetchWithEfCore(ContextProfile profile, Guid id, CancellationToken ct)
    {
        // Delegate to existing TemplateEngineService implementation
        // This method uses the existing ShapeData logic
        object? entity = profile.RootEntity switch
        {
            "SampleInvoice" => await GetSampleInvoiceAsync(id, profile, ct),
            _ => throw new NotSupportedException($"Entity type '{profile.RootEntity}' not supported")
        };

        if (entity == null)
            return null;

        return ShapeData(entity, profile, "");
    }

    private async Task<SampleInvoice?> GetSampleInvoiceAsync(Guid id, ContextProfile profile, CancellationToken ct)
    {
        var query = _db.SampleInvoices.AsQueryable();

        // Apply includes based on profile
        foreach (var includePath in profile.IncludePaths)
        {
            query = query.Include(includePath);
        }

        return await query.FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    private object ShapeData(object entity, ContextProfile profile, string pathPrefix)
    {
        if (entity == null) return new { };

        var entityType = entity.GetType();
        var result = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;

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

    private IDbConnection CreateConnection()
    {
        // For in-memory database, we need to use the same connection
        // In production, this would create a new SQL Server or PostgreSQL connection
        var connection = _db.Database.GetDbConnection();
        return connection;
    }
}
