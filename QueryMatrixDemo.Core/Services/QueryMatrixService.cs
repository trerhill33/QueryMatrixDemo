using QueryMatrixDemo.Core.Interfaces;
using QueryMatrixDemo.Core.Models;
using QueryMatrixDemo.Core.Operators;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace QueryMatrixDemo.Core.Services;

public class QueryMatrixService : IQueryMatrixService
{
    public IQueryable<T> ApplyMatrix<T>(IQueryable<T> query, QueryMatrix matrix)
    {
        var expression = BuildExpression<T>(matrix);
        return query.Where(expression);
    }

    public Expression<Func<T, bool>> BuildExpression<T>(QueryMatrix matrix)
    {
        // Create parameter expression (e.g., x => ...)
        var parameter = Expression.Parameter(typeof(T), "x");

        // Build the expression tree
        var expression = BuildMatrixExpression<T>(matrix, parameter);

        // Create lambda expression
        return Expression.Lambda<Func<T, bool>>(expression, parameter);
    }

    private Expression BuildMatrixExpression<T>(QueryMatrix matrix, ParameterExpression parameter)
    {
        var expressions = new List<Expression>();

        // Build expressions for all conditions
        foreach (var condition in matrix.Conditions)
        {
            expressions.Add(BuildConditionExpression<T>(condition, parameter));
        }

        // Build expressions for nested matrices
        foreach (var nestedMatrix in matrix.NestedMatrices)
        {
            expressions.Add(BuildMatrixExpression<T>(nestedMatrix, parameter));
        }

        if (!expressions.Any())
        {
            return Expression.Constant(true);
        }

        // Combine expressions based on logical operator
        return CombineExpressions(expressions, matrix.LogicalOperator);
    }

    private Expression BuildConditionExpression<T>(QueryCondition condition, ParameterExpression parameter)
    {
        var property = Expression.Property(parameter, condition.Field);
        var propertyType = typeof(T).GetProperty(condition.Field)!.PropertyType;

        // Handle column comparison
        if (condition.Operator.IsColumnOperation && !string.IsNullOrEmpty(condition.CompareToColumn))
        {
            var otherProperty = Expression.Property(parameter, condition.CompareToColumn);
            return BuildColumnComparisonExpression(property, otherProperty, condition.Operator);
        }

        // Handle null checks
        if (condition.Operator.IsNullOperation)
        {
            return Expression.Equal(property, Expression.Constant(null, propertyType));
        }

        // Convert value to property type
        var convertedValue = ConvertValue(condition.Value.Value, propertyType);
        var constantExpression = Expression.Constant(convertedValue, propertyType);

        // Build appropriate comparison expression
        return BuildComparisonExpression(property, constantExpression, condition.Operator);
    }

    private Expression BuildComparisonExpression(Expression property, Expression value, QueryOperator op)
    {
        return op.Value switch
        {
            "_eq" => Expression.Equal(property, value),
            "_neq" => Expression.NotEqual(property, value),
            "_gt" => Expression.GreaterThan(property, value),
            "_lt" => Expression.LessThan(property, value),
            "_gte" => Expression.GreaterThanOrEqual(property, value),
            "_lte" => Expression.LessThanOrEqual(property, value),
            "_like" => BuildStringComparisonExpression(property, value, StringComparison.Ordinal),
            "_ilike" => BuildStringComparisonExpression(property, value, StringComparison.OrdinalIgnoreCase),
            "_in" => BuildInExpression(property, value),
            "_nin" => Expression.Not(BuildInExpression(property, value)),
            _ => throw new NotSupportedException($"Operator {op.Value} is not supported")
        };
    }

    private Expression BuildColumnComparisonExpression(Expression property1, Expression property2, QueryOperator op)
    {
        return op.Value switch
        {
            "_ceq" => Expression.Equal(property1, property2),
            "_cne" => Expression.NotEqual(property1, property2),
            "_cgt" => Expression.GreaterThan(property1, property2),
            "_clt" => Expression.LessThan(property1, property2),
            "_cgte" => Expression.GreaterThanOrEqual(property1, property2),
            "_clte" => Expression.LessThanOrEqual(property1, property2),
            _ => throw new NotSupportedException($"Column operator {op.Value} is not supported")
        };
    }

    private Expression BuildStringComparisonExpression(Expression property, Expression pattern, StringComparison comparison)
    {
        // Default to simple string.Contains if no specific StringComparison is needed
        if (comparison == StringComparison.Ordinal || comparison == StringComparison.InvariantCulture)
        {
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
            return Expression.Call(property, containsMethod, pattern);
        }

        // Handle case-insensitive and other comparisons explicitly
        if (comparison == StringComparison.OrdinalIgnoreCase || comparison == StringComparison.InvariantCultureIgnoreCase)
        {
            // Use ToLowerInvariant for case-insensitive Contains
            var toLowerMethod = typeof(string).GetMethod("ToLowerInvariant")!;
            var propertyLower = Expression.Call(property, toLowerMethod);
            var patternLower = Expression.Call(pattern, toLowerMethod);

            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
            return Expression.Call(propertyLower, containsMethod, patternLower);
        }

        // Default fallback if unsupported
        throw new NotSupportedException($"The comparison type {comparison} is not supported.");
    }

    private Expression BuildInExpression(Expression property, Expression values)
    {
        if (values is ConstantExpression constantExp && constantExp.Value is IEnumerable enumerable)
        {
            var method = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                .MakeGenericMethod(property.Type);

            return Expression.Call(null, method, Expression.Constant(enumerable), property);
        }

        throw new NotSupportedException("Invalid values for IN operator");
    }

    private Expression CombineExpressions(List<Expression> expressions, QueryOperator logicalOperator)
    {
        if (expressions.Count == 0)
            return Expression.Constant(true);

        var result = expressions[0];
        for (var i = 1; i < expressions.Count; i++)
        {
            result = logicalOperator.Value switch
            {
                "_and" => Expression.AndAlso(result, expressions[i]),
                "_or" => Expression.OrElse(result, expressions[i]),
                "_not" => Expression.Not(result),
                _ => throw new NotSupportedException($"Logical operator {logicalOperator.Value} is not supported")
            };
        }

        return result;
    }

    private object? ConvertValue(object? value, Type targetType)
    {
        if (value == null)
            return null;

        // Handle JsonElement
        if (value is JsonElement jsonElement)
        {
            return ConvertJsonElement(jsonElement, targetType);
        }

        // Handle nullable types
        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            targetType = Nullable.GetUnderlyingType(targetType)!;
        }

        // Handle enums
        if (targetType.IsEnum)
        {
            if (value is string stringValue)
                return Enum.Parse(targetType, stringValue, ignoreCase: true);
            if (value is int intValue)
                return Enum.ToObject(targetType, intValue);

            throw new InvalidCastException($"Cannot convert value '{value}' to enum type '{targetType.Name}'.");
        }

        // Handle collections for IN/NOT IN operators
        if (targetType.IsArray && value is IEnumerable<object> enumerable)
        {
            var elementType = targetType.GetElementType() 
                ?? throw new InvalidOperationException("Cannot determine the element type of the target array.");
            var array = enumerable
                .Select(item => ConvertValue(item, elementType))
                .ToArray();

            var typedArray = Array.CreateInstance(elementType, array.Length);
            Array.Copy(array, typedArray, array.Length);
            return typedArray;
        }

        // Handle standard type conversions
        if (value is IConvertible)
        {
            try
            {
                return Convert.ChangeType(value, targetType);
            }
            catch (InvalidCastException)
            {
                throw new InvalidCastException($"Value '{value}' cannot be converted to type '{targetType.Name}'.");
            }
        }
        Console.WriteLine($"Value: {value}, Value Type: {value?.GetType()}, Target Type: {targetType}");
        var ss = value?.GetType();


        // Can't figure it out
        throw new InvalidCastException($"Value '{value}' must implement IConvertible to convert to type '{targetType.Name}'.");
    }

    private object? ConvertJsonElement(JsonElement jsonElement, Type targetType)
    {
        if (targetType == typeof(string))
        {
            return jsonElement.GetString();
        }
        if (targetType == typeof(int))
        {
            return jsonElement.TryGetInt32(out var result) ? result : throw new InvalidCastException($"Cannot convert '{jsonElement}' to int.");
        }
        if (targetType == typeof(long))
        {
            return jsonElement.TryGetInt64(out var result) ? result : throw new InvalidCastException($"Cannot convert '{jsonElement}' to long.");
        }
        if (targetType == typeof(bool))
        {
            if (jsonElement.ValueKind == JsonValueKind.True || jsonElement.ValueKind == JsonValueKind.False)
            {
                return jsonElement.GetBoolean();
            }
            throw new InvalidCastException($"Cannot convert '{jsonElement}' to bool. Value is not a boolean.");
        }
        if (targetType == typeof(double))
        {
            return jsonElement.TryGetDouble(out var result) ? result : throw new InvalidCastException($"Cannot convert '{jsonElement}' to double.");
        }
        if (targetType == typeof(DateTime))
        {
            return jsonElement.TryGetDateTime(out var result) ? result : throw new InvalidCastException($"Cannot convert '{jsonElement}' to DateTime.");
        }

        throw new InvalidCastException($"JsonElement type '{jsonElement.ValueKind}' cannot be converted to '{targetType.Name}'.");
    }

    public IEnumerable<PropertyInfo> GetFilterableProperties<T>()
    {
        return typeof(T).GetProperties()
            .Where(p => IsFilterableType(p.PropertyType));
    }

    public IEnumerable<QueryOperator> GetValidOperatorsForProperty(PropertyInfo property)
    {
        if (property == null)
            return [];

        var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

        return QueryOperator.All.Where(op => IsOperatorValidForType(op, type));
    }

    private bool IsFilterableType(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            type = Nullable.GetUnderlyingType(type)!;
        }

        return type.IsPrimitive
            || type == typeof(string)
            || type == typeof(DateTime)
            || type == typeof(decimal)
            || type == typeof(Guid)
            || type.IsEnum;
    }

    private bool IsOperatorValidForType(QueryOperator op, Type propertyType)
    {
        // Handle null operations
        if (op.IsNullOperation)
        {
            return true; // Null operations are valid for all types
        }

        // Handle text operations
        if (op.IsTextOperation)
        {
            return propertyType == typeof(string);
        }

        // Handle column operations
        if (op.IsColumnOperation)
        {
            return true; // Column operations are valid for all types when comparing same types
        }

        // Handle logical operations
        if (op.IsLogicalOperation)
        {
            return true;
        }

        // Handle comparison operators based on type
        return op.Value switch
        {
            "_eq" or "_neq" => true,// Equals and Not Equals work for all types
            "_gt" or "_lt" or "_gte" or "_lte" => IsComparableType(propertyType),
            "_in" or "_nin" => true,// In and Not In work for all types
            _ => false,
        };
    }

    private bool IsComparableType(Type type)
    {
        return type == typeof(int)
            || type == typeof(long)
            || type == typeof(float)
            || type == typeof(double)
            || type == typeof(decimal)
            || type == typeof(DateTime)
            || type == typeof(TimeSpan)
            || type == typeof(char)
            || type == typeof(byte)
            || type == typeof(sbyte)
            || type == typeof(short)
            || type == typeof(ushort)
            || type == typeof(uint)
            || type == typeof(ulong)
            || type.IsEnum;
    }
}