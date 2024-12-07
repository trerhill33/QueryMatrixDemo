using QueryMatrixDemo.Core.Core.Caching;
using QueryMatrixDemo.Core.Core.Models;
using QueryMatrixDemo.Core.Core.Operators;
using QueryMatrixDemo.Core.Core.Operators.Providers;
using QueryMatrixDemo.Core.Core.Services.Interfaces;
using System.Linq.Expressions;

namespace QueryMatrixDemo.Core.Core.Services;

/// <summary>
/// Builds LINQ expressions based on a provided QueryMatrix that can be used to filter data sources based on the defined conditions and logical operators.
/// </summary>
public class QueryExpressionBuilder : IQueryExpressionBuilder
{
    private readonly IOperatorStrategyProvider _operatorStrategyProvider;
    private readonly IValueConverter _valueConverter;
    private readonly IPropertyInfoCache _propertyInfoCache;

    public QueryExpressionBuilder(
        IOperatorStrategyProvider operatorStrategyProvider,
        IValueConverter valueConverter,
        IPropertyInfoCache propertyInfoCache)
    {
        _operatorStrategyProvider = operatorStrategyProvider ?? throw new ArgumentNullException(nameof(operatorStrategyProvider));
        _valueConverter = valueConverter ?? throw new ArgumentNullException(nameof(valueConverter));
        _propertyInfoCache = propertyInfoCache ?? throw new ArgumentNullException(nameof(propertyInfoCache));
    }

    /// <summary>
    /// Builds a LINQ expression from the provided QueryMatrix.
    /// </summary>
    public Expression<Func<T, bool>> BuildExpression<T>(QueryMatrix matrix)
    {
        ArgumentNullException.ThrowIfNull(matrix);

        var parameter = Expression.Parameter(typeof(T), "x");
        var expressionBody = BuildMatrixExpression<T>(matrix, parameter);
        return Expression.Lambda<Func<T, bool>>(expressionBody, parameter);
    }

    /// <summary>
    /// Recursively builds the expression for a QueryMatrix.
    /// </summary>
    private Expression BuildMatrixExpression<T>(QueryMatrix matrix, ParameterExpression parameter)
    {
        var expressions = matrix.Conditions
            .Select(condition => BuildConditionExpression<T>(condition, parameter))
            .Concat(matrix.NestedMatrices.Select(nested => BuildMatrixExpression<T>(nested, parameter)))
            .ToList();

        return expressions.Count != 0
            ? CombineExpressions(expressions, matrix.LogicalOperator)
            : Expression.Constant(true);
    }

    /// <summary>
    /// Builds an expression for a single QueryCondition.
    /// </summary>
    private Expression BuildConditionExpression<T>(QueryCondition condition, ParameterExpression parameter)
    {
        ArgumentNullException.ThrowIfNull(condition);

        // Retrieve PropertyInfo from cache
        var propertyInfo = _propertyInfoCache.GetProperty<T>(condition.Field)
            ?? throw new ArgumentException($"Property '{condition.Field}' does not exist on type '{typeof(T).Name}'.");

        var property = Expression.Property(parameter, propertyInfo);

        // Handle column-to-column comparisons
        if (condition.Operator.IsColumnOperation)
        {
            if (string.IsNullOrWhiteSpace(condition.CompareToColumn))
                throw new ArgumentException("CompareToColumn cannot be null or whitespace for column operations.", nameof(condition.CompareToColumn));

            var comparePropertyInfo = _propertyInfoCache.GetProperty<T>(condition.CompareToColumn);
            var compareProperty = Expression.Property(parameter, comparePropertyInfo);

            var columnStrategy = _operatorStrategyProvider.GetColumnStrategy(condition.Operator);
            return columnStrategy.BuildColumnExpression(property, compareProperty);
        }

        // Handle null checks
        if (condition.Operator.IsNullOperation)
        {
            return Expression.Equal(property, Expression.Constant(null, property.Type));
        }

        // Handle value-based comparisons
        var valueStrategy = _operatorStrategyProvider.GetValueStrategy(condition.Operator);
        var convertedValue = _valueConverter.ConvertValue(condition.Value.Value, property.Type);
        var constantExpression = Expression.Constant(convertedValue, property.Type);

        return valueStrategy.BuildValueExpression(property, constantExpression);
    }

    /// <summary>
    /// Combines multiple expressions using the specified logical operator.
    /// </summary>
    private static Expression CombineExpressions(IEnumerable<Expression> expressions, QueryOperator logicalOperator)
        => logicalOperator.Value switch
        {
            "_and" => expressions.Aggregate(Expression.AndAlso),
            "_or" => expressions.Aggregate(Expression.OrElse),
            "_not" when expressions.Count() == 1 => Expression.Not(expressions.Single()),
            _ => throw new NotSupportedException($"Logical operator '{logicalOperator.Value}' is not supported.")
        };
}
