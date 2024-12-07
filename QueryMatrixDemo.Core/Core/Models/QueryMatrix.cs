using QueryMatrixDemo.Core.Core.Operators;

namespace QueryMatrixDemo.Core.Core.Models;

/// <summary>
/// Represents a matrix of query conditions combined with logical operators, containing individual QueryCondition objects and nested QueryMatrix instances for grouped conditions.
/// </summary>
public sealed record QueryMatrix
{
    public required QueryOperator LogicalOperator { get; init; } = QueryOperator.And;
    public required IReadOnlyCollection<QueryCondition> Conditions { get; init; } = [];
    public required IReadOnlyCollection<QueryMatrix> NestedMatrices { get; init; } = [];
}
