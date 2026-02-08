using System.Text.RegularExpressions;
using PdfTemplateSystem.Models;

namespace PdfTemplateSystem.Services;

public interface ICustomJoinValidator
{
    Task<ValidationResult> ValidateAsync(CustomJoin join, Dictionary<string, object> schema);
    Task<ValidationResult> ValidateAllAsync(List<CustomJoin> joins, Dictionary<string, object> schema);
}

public class CustomJoinValidator : ICustomJoinValidator
{
    private const int MaxJoinDepth = 3;
    private const int MaxJoinsPerContext = 10;

    public async Task<ValidationResult> ValidateAsync(CustomJoin join, Dictionary<string, object> schema)
    {
        var errors = new List<string>();

        // 1. Valid alias (SQL identifier)
        if (!Regex.IsMatch(join.Alias, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
            errors.Add($"Invalid alias '{join.Alias}'. Use only letters, numbers, and underscores.");

        // 2. Valid join type
        var validJoinTypes = new[] { "LEFT", "INNER", "RIGHT" };
        if (!validJoinTypes.Contains(join.JoinType.ToUpper()))
            errors.Add($"Invalid join type '{join.JoinType}'. Use LEFT, INNER, or RIGHT.");

        // 3. Valid operator
        var validOps = new[] { "=", "!=", ">", "<", ">=", "<=", "LIKE" };
        if (!validOps.Contains(join.Condition.Operator))
            errors.Add($"Invalid operator '{join.Condition.Operator}'");

        // 4. Check join depth
        var depth = CalculateJoinDepth(join);
        if (depth > MaxJoinDepth)
            errors.Add($"Join depth {depth} exceeds maximum of {MaxJoinDepth}");

        // 5. Validate nested joins recursively
        foreach (var nested in join.NestedJoins)
        {
            var nestedResult = await ValidateAsync(nested, schema);
            errors.AddRange(nestedResult.Errors);
        }

        return new ValidationResult(errors);
    }

    public async Task<ValidationResult> ValidateAllAsync(List<CustomJoin> joins, Dictionary<string, object> schema)
    {
        var errors = new List<string>();

        // Check max joins limit
        var totalJoins = CountTotalJoins(joins);
        if (totalJoins > MaxJoinsPerContext)
            errors.Add($"Total joins ({totalJoins}) exceeds maximum of {MaxJoinsPerContext}");

        // Validate each join
        foreach (var join in joins)
        {
            var result = await ValidateAsync(join, schema);
            errors.AddRange(result.Errors);
        }

        return new ValidationResult(errors);
    }

    private int CalculateJoinDepth(CustomJoin join, int current = 1)
    {
        if (!join.NestedJoins.Any()) return current;
        return join.NestedJoins.Max(n => CalculateJoinDepth(n, current + 1));
    }

    private int CountTotalJoins(List<CustomJoin> joins)
    {
        var count = joins.Count;
        foreach (var join in joins)
        {
            count += CountTotalJoins(join.NestedJoins);
        }
        return count;
    }
}
