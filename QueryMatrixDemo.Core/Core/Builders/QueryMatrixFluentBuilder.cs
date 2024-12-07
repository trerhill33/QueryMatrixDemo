using QueryMatrixDemo.Core.Core.Models;
using QueryMatrixDemo.Core.Core.Operators;
using QueryMatrixDemo.Core.Core.Utilities;

namespace QueryMatrixDemo.Core.Core.Builders;

/// <summary>
/// Implements the Builder Pattern, providing a fluent interface to construct QueryMatrix objects by adding conditions, specifying logical operators, and nesting matrices.
/// </summary>
public class QueryMatrixFluentBuilder : IQueryMatrixFluentBuilder
{
    private readonly List<QueryCondition> _conditions = [];
    private readonly List<QueryMatrix> _nestedMatrices = [];
    private QueryOperator _logicalOperator = QueryOperator.And;

    public IQueryMatrixFluentBuilder WithLogicalOperator(QueryOperator op)
    {
        _logicalOperator = op;
        return this;
    }

    public IQueryMatrixFluentBuilder AddCondition(string field, QueryOperator op, object value)
    {
        _conditions.Add(new QueryCondition
        {
            Field = field,
            Operator = op,
            Value = QueryValue.Single(value)
        });
        return this;
    }

    public IQueryMatrixFluentBuilder AddCondition(string field, QueryOperator op, string pattern)
    {
        _conditions.Add(new QueryCondition
        {
            Field = field,
            Operator = op,
            Value = QueryValue.Pattern(pattern)
        });
        return this;
    }

    public IQueryMatrixFluentBuilder AddCondition(string field, QueryOperator op, IEnumerable<object> values)
    {
        _conditions.Add(new QueryCondition
        {
            Field = field,
            Operator = op,
            Value = QueryValue.Array(values)
        });
        return this;
    }

    public IQueryMatrixFluentBuilder AddNestedMatrix(QueryMatrix nestedMatrix)
    {
        _nestedMatrices.Add(nestedMatrix);
        return this;
    }

    public QueryMatrix Build()
    {
        return new QueryMatrix
        {
            LogicalOperator = _logicalOperator,
            Conditions = _conditions.AsReadOnly(),
            NestedMatrices = _nestedMatrices.AsReadOnly()
        };
    }
}
