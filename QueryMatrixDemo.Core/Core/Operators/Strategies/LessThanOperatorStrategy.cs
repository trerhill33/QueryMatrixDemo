using System.Linq.Expressions;
using QueryMatrixDemo.Core.Core.Operators.Strategies.Interfaces;

namespace QueryMatrixDemo.Core.Core.Operators.Strategies;

public class LessThanOperatorStrategy : IValueOperatorStrategy, IColumnOperatorStrategy
{
    public bool CanHandle(QueryOperator op) =>
        op == QueryOperator.LessThan || op == QueryOperator.ColumnLessThan;

    public Expression BuildValueExpression(Expression property, Expression value)
    {
        return Expression.LessThan(property, value);
    }

    public Expression BuildColumnExpression(Expression property1, Expression property2)
    {
        return Expression.LessThan(property1, property2);
    }
}
