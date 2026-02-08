using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PdfTemplateSystem.Data;
using PdfTemplateSystem.Models;

namespace PdfTemplateSystem.Services;

public class DynamicSqlQueryBuilder
{
    private readonly ApplicationDbContext _context;

    public DynamicSqlQueryBuilder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(string Sql, object Parameters)> BuildQueryAsync(
        ContextProfile profile,
        Guid id)
    {
        var sql = new StringBuilder();
        var aliases = new Dictionary<string, string>();
        var model = _context.Model;

        // 1. Build SELECT clause from AllowedFields
        sql.AppendLine("SELECT");
        var selectClauses = BuildSelectClauses(profile.AllowedFields, aliases, model, profile.RootEntity);
        sql.AppendLine("    " + string.Join(",\n    ", selectClauses));

        // 2. FROM clause (root entity)
        var rootTable = GetTableName(profile.RootEntity, model);
        sql.AppendLine($"FROM {rootTable} AS root");
        aliases["root"] = profile.RootEntity;

        // 3. Standard EF Include paths as JOINs
        foreach (var includePath in profile.IncludePaths)
        {
            var joinSql = BuildEfJoin(includePath, profile.RootEntity, model, aliases);
            sql.Append(joinSql);
        }

        // 4. Custom joins
        foreach (var customJoin in profile.CustomJoins)
        {
            var joinSql = BuildCustomJoinRecursive(customJoin, model, aliases);
            sql.Append(joinSql);
        }

        // 5. WHERE clause with parameter
        sql.AppendLine($"WHERE root.{EscapeIdentifier("Id")} = @Id");

        return (sql.ToString(), new { Id = id });
    }

    private string BuildCustomJoinRecursive(
        CustomJoin join,
        Microsoft.EntityFrameworkCore.Metadata.IModel model,
        Dictionary<string, string> aliases)
    {
        var targetTable = GetTableName(join.TargetEntity, model);
        var joinAlias = GenerateSafeAlias(join.Alias);
        aliases[joinAlias] = join.TargetEntity;

        // Resolve left side (could be root, included table, or another custom join)
        var leftAlias = ResolveFieldAlias(join.Condition.LeftField, aliases);
        var leftColumn = GetColumnName(join.Condition.LeftField);
        var rightColumn = GetColumnName(join.Condition.RightField);

        // Validate SQL injection prevention
        ValidateIdentifier(leftColumn);
        ValidateIdentifier(rightColumn);
        ValidateOperator(join.Condition.Operator);

        var sql = new StringBuilder();
        sql.AppendLine($"{join.JoinType} JOIN {targetTable} AS {joinAlias}");
        sql.AppendLine($"    ON {leftAlias}.{EscapeIdentifier(leftColumn)} {join.Condition.Operator} {joinAlias}.{EscapeIdentifier(rightColumn)}");

        // Handle nested joins
        foreach (var nested in join.NestedJoins)
        {
            sql.Append(BuildCustomJoinRecursive(nested, model, aliases));
        }

        return sql.ToString();
    }

    private List<string> BuildSelectClauses(
        List<string> allowedFields,
        Dictionary<string, string> aliases,
        Microsoft.EntityFrameworkCore.Metadata.IModel model,
        string rootEntity)
    {
        var clauses = new List<string>();

        // Always include root Id
        clauses.Add($"root.{EscapeIdentifier("Id")} AS root_Id");

        foreach (var field in allowedFields)
        {
            var parts = field.Split('.');
            string alias;

            if (parts.Length == 1)
            {
                alias = "root";
            }
            else if (parts[0].EndsWith("[]"))
            {
                // Collection field like "Items[].Description"
                alias = parts[0].TrimEnd('[', ']');
            }
            else
            {
                // Custom join alias or navigation property
                alias = parts[0];
            }

            var column = parts[^1];
            ValidateIdentifier(column);
            var safeColumn = EscapeIdentifier(column);
            var resultAlias = string.Join("_", parts).Replace("[]", "");

            clauses.Add($"{alias}.{safeColumn} AS {EscapeIdentifier(resultAlias)}");
        }

        return clauses;
    }

    private string BuildEfJoin(
        string includePath,
        string rootEntity,
        Microsoft.EntityFrameworkCore.Metadata.IModel model,
        Dictionary<string, string> aliases)
    {
        var entityType = model.FindEntityType($"PdfTemplateSystem.Models.{rootEntity}");
        if (entityType == null)
            throw new InvalidOperationException($"Entity type '{rootEntity}' not found");

        var navigation = entityType.GetNavigations().FirstOrDefault(n => n.Name == includePath);
        if (navigation == null)
            throw new InvalidOperationException($"Navigation '{includePath}' not found on '{rootEntity}'");

        var targetTable = GetTableName(navigation.TargetEntityType.ClrType.Name, model);
        var joinAlias = includePath;
        aliases[joinAlias] = navigation.TargetEntityType.ClrType.Name;

        // Get foreign key
        var fk = navigation.ForeignKey;
        var fkProperty = fk.Properties.FirstOrDefault();
        
        if (fkProperty == null)
            throw new InvalidOperationException($"Foreign key not found for navigation '{includePath}'");

        var sql = new StringBuilder();
        
        // Determine if this is a one-to-many or many-to-one relationship
        if (navigation.IsCollection)
        {
            // One-to-many: JOIN target ON root.Id = target.ForeignKeyColumn
            var principalKey = fk.PrincipalKey.Properties.FirstOrDefault()?.Name ?? "Id";
            sql.AppendLine($"LEFT JOIN {targetTable} AS {joinAlias} ON root.{EscapeIdentifier(principalKey)} = {joinAlias}.{EscapeIdentifier(fkProperty.Name)}");
        }
        else
        {
            // Many-to-one: JOIN target ON root.ForeignKeyColumn = target.Id
            var principalKey = fk.PrincipalKey.Properties.FirstOrDefault()?.Name ?? "Id";
            sql.AppendLine($"LEFT JOIN {targetTable} AS {joinAlias} ON root.{EscapeIdentifier(fkProperty.Name)} = {joinAlias}.{EscapeIdentifier(principalKey)}");
        }

        return sql.ToString();
    }

    private string GetTableName(string entityName, Microsoft.EntityFrameworkCore.Metadata.IModel model)
    {
        var entityType = model.FindEntityType($"PdfTemplateSystem.Models.{entityName}");
        if (entityType == null)
            throw new InvalidOperationException($"Entity type '{entityName}' not found");

        // For EF Core 8, use StoreObjectIdentifier to get table name
        var tableName = entityType.GetTableName();
        if (string.IsNullOrEmpty(tableName))
        {
            // Fallback to entity name if table name is not configured
            tableName = entityType.ClrType.Name;
        }

        return EscapeIdentifier(tableName);
    }

    private string ResolveFieldAlias(string field, Dictionary<string, string> aliases)
    {
        var parts = field.Split('.');
        if (parts.Length == 1)
            return "root";

        var potentialAlias = parts[0].TrimEnd('[', ']');
        return aliases.ContainsKey(potentialAlias) ? potentialAlias : "root";
    }

    private string GetColumnName(string field)
    {
        var parts = field.Split('.');
        return parts[^1];
    }

    private string GenerateSafeAlias(string alias)
    {
        ValidateIdentifier(alias);
        return alias;
    }

    // Security: SQL injection prevention
    private void ValidateIdentifier(string identifier)
    {
        if (!Regex.IsMatch(identifier, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
            throw new SecurityException($"Invalid SQL identifier: {identifier}");
    }

    private void ValidateOperator(string op)
    {
        var allowed = new[] { "=", "!=", ">", "<", ">=", "<=", "LIKE" };
        if (!allowed.Contains(op))
            throw new SecurityException($"Invalid operator: {op}");
    }

    private string EscapeIdentifier(string identifier)
    {
        ValidateIdentifier(identifier);
        // Use square brackets for SQL Server / double quotes for PostgreSQL
        // For in-memory database, square brackets work
        return $"[{identifier}]";
    }
}
