using QueryMatrixDemo.Core.Models;
using QueryMatrixDemo.Core.Operators;

namespace QueryMatrixDemo.Core.Builders;

public sealed class QueryMatrixBuilder
{
    private readonly List<QueryCondition> _conditions = [];
    private readonly List<QueryMatrix> _nestedMatrices = [];
    private QueryOperator _logicalOperator = QueryOperator.And;

    public QueryMatrixBuilder WithLogicalOperator(QueryOperator op)
    {
        if (!op.IsLogicalOperation)
            throw new ArgumentException("Operator must be a logical operator (AND/OR/NOT)", nameof(op));
            
        _logicalOperator = op;
        return this;
    }

    public QueryMatrixBuilder AddCondition(string field, QueryOperator op, object value)
    {
        if (string.IsNullOrWhiteSpace(field))
            throw new ArgumentException("Field name cannot be null or whitespace.", nameof(field));

        if (value == null)
            throw new ArgumentNullException(nameof(value), "Value cannot be null.");

        _conditions.Add(new QueryCondition
        {
            Field = field,
            Operator = op,
            Value = ConditionValue.Single(value)
        });
        return this;
    }

    public QueryMatrixBuilder AddColumnComparison(string field, QueryOperator op, string compareToColumn)
    {
        if (!op.IsColumnOperation)
            throw new ArgumentException("Operator must be a column operation", nameof(op));

        _conditions.Add(new QueryCondition
        {
            Field = field,
            Operator = op,
            Value = ConditionValue.Column(compareToColumn),
            CompareToColumn = compareToColumn
        });
        return this;
    }

    public QueryMatrixBuilder AddNestedMatrix(QueryMatrix matrix)
    {
        _nestedMatrices.Add(matrix);
        return this;
    }

    public QueryMatrix Build() => new()
    {
        LogicalOperator = _logicalOperator,
        Conditions = _conditions,
        NestedMatrices = _nestedMatrices
    };
}
