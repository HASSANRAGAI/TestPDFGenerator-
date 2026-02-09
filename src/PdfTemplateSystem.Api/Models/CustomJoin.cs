namespace PdfTemplateSystem.Models;

public class CustomJoin
{
    public string Alias { get; set; } = string.Empty;
    public string TargetEntity { get; set; } = string.Empty;
    public string JoinType { get; set; } = "LEFT";
    public JoinCondition Condition { get; set; } = new();
    public List<CustomJoin> NestedJoins { get; set; } = new();

    public override bool Equals(object? obj)
    {
        if (obj is not CustomJoin other) return false;
        
        return Alias == other.Alias &&
               TargetEntity == other.TargetEntity &&
               JoinType == other.JoinType &&
               Condition.Equals(other.Condition) &&
               NestedJoins.SequenceEqual(other.NestedJoins);
    }

    public override int GetHashCode()
    {
        var hash = HashCode.Combine(Alias, TargetEntity, JoinType, Condition);
        foreach (var nested in NestedJoins)
        {
            hash = HashCode.Combine(hash, nested);
        }
        return hash;
    }
}

public class JoinCondition
{
    public string LeftField { get; set; } = string.Empty;
    public string RightField { get; set; } = string.Empty;
    public string Operator { get; set; } = "=";

    public override bool Equals(object? obj)
    {
        if (obj is not JoinCondition other) return false;
        
        return LeftField == other.LeftField &&
               RightField == other.RightField &&
               Operator == other.Operator;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(LeftField, RightField, Operator);
    }
}

public class ValidationResult
{
    public List<string> Errors { get; set; } = new();
    public bool IsValid => !Errors.Any();
    
    public ValidationResult() { }
    
    public ValidationResult(List<string> errors)
    {
        Errors = errors;
    }
}
