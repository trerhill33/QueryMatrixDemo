using System.Linq.Expressions;
using QueryMatrixDemo.Core.Core.Operators.Strategies.Interfaces;

namespace QueryMatrixDemo.Core.Core.Operators.Strategies;

public class EqualsOperatorStrategy : IValueOperatorStrategy, IColumnOperatorStrategy
{
    public bool CanHandle(QueryOperator op) => op == QueryOperator.Equal || op == QueryOperator.ColumnEqual;

    public Expression BuildValueExpression(Expression property, Expression value)
    {
        return Expression.Equal(property, value);
    }

    public Expression BuildColumnExpression(Expression property1, Expression property2)
    {
        return Expression.Equal(property1, property2);
    }
}
