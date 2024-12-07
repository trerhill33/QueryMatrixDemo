using System.Linq.Expressions;
using QueryMatrixDemo.Core.Core.Operators.Strategies.Interfaces;

namespace QueryMatrixDemo.Core.Core.Operators.Strategies;

public class NotEqualOperatorStrategy : IValueOperatorStrategy, IColumnOperatorStrategy
{
    public bool CanHandle(QueryOperator op) =>
        op == QueryOperator.NotEqual || op == QueryOperator.ColumnNotEqual;

    public Expression BuildValueExpression(Expression property, Expression value)
    {
        return Expression.NotEqual(property, value);
    }

    public Expression BuildColumnExpression(Expression property1, Expression property2)
    {
        return Expression.NotEqual(property1, property2);
    }
}
