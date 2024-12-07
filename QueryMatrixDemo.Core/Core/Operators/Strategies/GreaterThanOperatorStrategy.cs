using System.Linq.Expressions;
using QueryMatrixDemo.Core.Core.Operators.Strategies.Interfaces;

namespace QueryMatrixDemo.Core.Core.Operators.Strategies;

public class GreaterThanOperatorStrategy : IValueOperatorStrategy, IColumnOperatorStrategy
{
    public bool CanHandle(QueryOperator op) =>
        op == QueryOperator.GreaterThan || op == QueryOperator.ColumnGreaterThan;

    public Expression BuildValueExpression(Expression property, Expression value)
    {
        return Expression.GreaterThan(property, value);
    }

    public Expression BuildColumnExpression(Expression property1, Expression property2)
    {
        return Expression.GreaterThan(property1, property2);
    }
}
