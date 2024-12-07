using QueryMatrixDemo.Core.Core.Operators;
using QueryMatrixDemo.Core.Core.Operators.Strategies.Interfaces;
using System.Collections;
using System.Linq.Expressions;

namespace QueryMatrixDemo.Core.Core.Operators.Strategies;


public class InOperatorStrategy : IValueOperatorStrategy
{
    public bool CanHandle(QueryOperator op) =>
        op == QueryOperator.In;

    public Expression BuildValueExpression(Expression property, Expression value)
    {
        // Ensure the value is a collection
        if (value is not ConstantExpression constant || constant.Value is not IEnumerable enumerable)
            throw new ArgumentException("Value for 'In' operator must be an enumerable collection.", nameof(value));

        // Determine the type of the elements in the collection
        Type elementType = property.Type;

        // Convert the enumerable to the appropriate type
        var convertedEnumerable = Expression.Constant(
            enumerable.Cast<object>().Select(v => Convert.ChangeType(v, elementType)).ToList(),
            typeof(IEnumerable<object>)
        );

        // Get the generic 'Contains' method from Enumerable
        var containsMethod = typeof(Enumerable).GetMethods()
            .FirstOrDefault(m => m.Name == "Contains" && m.GetParameters().Length == 2)?
            .MakeGenericMethod(elementType);

        if (containsMethod == null)
        {
            throw new InvalidOperationException("Could not find 'Contains' method.");
        }

        // Build the 'Contains' method call expression
        var containsCall = Expression.Call(containsMethod, convertedEnumerable, property);

        return containsCall;
    }
}
