namespace TestPDFGenerator.Api.Models;

public class CustomJoin
{
    public string Alias { get; set; } = string.Empty;
    public string TargetEntity { get; set; } = string.Empty;
    public string JoinType { get; set; } = "LEFT";
    public JoinCondition Condition { get; set; } = new();
    public List<CustomJoin> NestedJoins { get; set; } = new();
}

public class JoinCondition
{
    public string LeftField { get; set; } = string.Empty;
    public string RightField { get; set; } = string.Empty;
    public string Operator { get; set; } = "=";
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
