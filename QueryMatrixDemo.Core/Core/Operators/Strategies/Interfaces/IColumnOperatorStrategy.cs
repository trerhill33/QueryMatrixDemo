using System.Linq.Expressions;

namespace QueryMatrixDemo.Core.Core.Operators.Strategies.Interfaces;

// Interface for strategies that handle column-to-column comparisons.
public interface IColumnOperatorStrategy
{
    bool CanHandle(QueryOperator op);
    Expression BuildColumnExpression(Expression property1, Expression property2);
}
