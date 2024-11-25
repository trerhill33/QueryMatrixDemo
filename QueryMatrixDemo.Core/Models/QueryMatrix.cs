using QueryMatrixDemo.Core.Operators;

namespace QueryMatrixDemo.Core.Models;

public sealed record QueryMatrix
{
    public required QueryOperator LogicalOperator { get; init; } = QueryOperator.And;
    public required IReadOnlyCollection<QueryCondition> Conditions { get; init; } = [];
    public required IReadOnlyCollection<QueryMatrix> NestedMatrices { get; init; } = [];
}
