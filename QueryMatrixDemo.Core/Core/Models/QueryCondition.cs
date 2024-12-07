using QueryMatrixDemo.Core.Core.Operators;

namespace QueryMatrixDemo.Core.Core.Models;

/// <summary>
/// Represents a single query condition, specifying the field, operator, value or column to compare against, and any additional parameters required for the condition.
/// </summary>
public sealed record QueryCondition
{
    public required string Field { get; init; }
    public required QueryOperator Operator { get; init; }
    public required QueryValue Value { get; init; }
    public string? CompareToColumn { get; init; }
}
