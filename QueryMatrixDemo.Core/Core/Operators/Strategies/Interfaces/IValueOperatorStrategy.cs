using System.Linq.Expressions;

namespace QueryMatrixDemo.Core.Core.Operators.Strategies.Interfaces;

// Interface for strategies that handle value-based comparisons.
public interface IValueOperatorStrategy
{
    bool CanHandle(QueryOperator op);
    Expression BuildValueExpression(Expression property, Expression value);
}
