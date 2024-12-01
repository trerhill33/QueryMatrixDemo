using QueryMatrixDemo.Core.Helpers;
using QueryMatrixDemo.Core.Interfaces;
using QueryMatrixDemo.Core.Models;
using QueryMatrixDemo.Core.Operators;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace QueryMatrixDemo.Core.Services
{
    public class QueryMatrixService : IQueryMatrixService
    {
        /// <summary>
        /// Applies the specified <see cref="QueryMatrix"/> to the given <see cref="IQueryable{T}"/>.
        /// </summary>
        public IQueryable<T> ApplyMatrix<T>(IQueryable<T> query, QueryMatrix matrix)
        {
            ArgumentNullException.ThrowIfNull(query);
            ArgumentNullException.ThrowIfNull(matrix);

            return query.Where(BuildExpression<T>(matrix));
        }

        /// <summary>
        /// Builds a LINQ expression from the provided <see cref="QueryMatrix"/>.
        /// </summary>
        public Expression<Func<T, bool>> BuildExpression<T>(QueryMatrix matrix)
        {
            ArgumentNullException.ThrowIfNull(matrix);

            var parameter = Expression.Parameter(typeof(T), "x");
            var expressionBody = BuildMatrixExpression<T>(matrix, parameter);
            return Expression.Lambda<Func<T, bool>>(expressionBody, parameter);
        }

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

        private Expression BuildConditionExpression<T>(QueryCondition condition, ParameterExpression parameter)
        {
            ArgumentNullException.ThrowIfNull(condition);

            var propertyInfo = typeof(T).GetProperty(condition.Field)
                ?? throw new ArgumentException($"Property '{condition.Field}' does not exist on type '{typeof(T).Name}'.");

            var property = Expression.Property(parameter, propertyInfo);
            var propertyType = propertyInfo.PropertyType;

            if (condition.Operator.IsColumnOperation && !string.IsNullOrWhiteSpace(condition.CompareToColumn))
            {
                var comparePropertyInfo = typeof(T).GetProperty(condition.CompareToColumn)
                    ?? throw new ArgumentException($"Property '{condition.CompareToColumn}' does not exist on type '{typeof(T).Name}'.");

                var otherProperty = Expression.Property(parameter, comparePropertyInfo);
                return BuildColumnComparisonExpression(property, otherProperty, condition.Operator);
            }

            if (condition.Operator.IsNullOperation)
            {
                return Expression.Equal(property, Expression.Constant(null, propertyType));
            }

            var convertedValue = ConvertValue(condition.Value.Value, propertyType);
            var constantExpression = Expression.Constant(convertedValue, propertyType);

            return BuildComparisonExpression(property, constantExpression, condition.Operator);
        }

        private static Expression BuildComparisonExpression(Expression property, Expression value, QueryOperator op)
        {
            return op.Value switch
            {
                "_eq" => Expression.Equal(property, value),
                "_neq" => Expression.NotEqual(property, value),
                "_gt" => Expression.GreaterThan(property, value),
                "_lt" => Expression.LessThan(property, value),
                "_gte" => Expression.GreaterThanOrEqual(property, value),
                "_lte" => Expression.LessThanOrEqual(property, value),
                "_like" => ExpressionHelper.BuildStringComparisonExpression(property, value, StringComparison.Ordinal),
                "_ilike" => ExpressionHelper.BuildStringComparisonExpression(property, value, StringComparison.OrdinalIgnoreCase),
                "_in" => ExpressionHelper.BuildInExpression(property, value),
                "_nin" => Expression.Not(ExpressionHelper.BuildInExpression(property, value)),
                _ => throw new NotSupportedException($"Operator '{op.Value}' is not supported.")
            };
        }

        private static Expression BuildColumnComparisonExpression(Expression property1, Expression property2, QueryOperator op)
            => op.Value switch
        {
            "_ceq" => Expression.Equal(property1, property2),
            "_cne" => Expression.NotEqual(property1, property2),
            "_cgt" => Expression.GreaterThan(property1, property2),
            "_clt" => Expression.LessThan(property1, property2),
            "_cgte" => Expression.GreaterThanOrEqual(property1, property2),
            "_clte" => Expression.LessThanOrEqual(property1, property2),
            _ => throw new NotSupportedException($"Column operator '{op.Value}' is not supported.")
        };

        private static Expression CombineExpressions(IEnumerable<Expression> expressions, QueryOperator logicalOperator)
            => logicalOperator.Value switch
        {
            "_and" => expressions.Aggregate(Expression.AndAlso),
            "_or" => expressions.Aggregate(Expression.OrElse),
            "_not" when expressions.Count() == 1 => Expression.Not(expressions.Single()),
            _ => throw new NotSupportedException($"Logical operator '{logicalOperator.Value}' is not supported.")
        };

        private object? ConvertValue(object? value, Type targetType)
        {
            if (value == null)
                return null;

            return value switch
            {
                JsonElement jsonElement => ConvertJsonElement(jsonElement, targetType),
                _ => ConvertGenericValue(value, targetType)
            };
        }

        private object? ConvertGenericValue(object value, Type targetType)
        {
            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            return underlyingType.IsEnum
                ? value switch
                {
                    string s => Enum.Parse(underlyingType, s, ignoreCase: true),
                    int i => Enum.ToObject(underlyingType, i),
                    _ => throw new InvalidCastException($"Cannot convert value '{value}' to enum type '{underlyingType.Name}'.")
                }
                : targetType.IsArray && value is IEnumerable enumerable
                    ? CreateTypedArray(enumerable, targetType)
                    : value is IConvertible
                        ? ConvertStandardType(value, underlyingType)
                        : throw new InvalidCastException($"Value '{value}' must implement IConvertible to convert to type '{underlyingType.Name}'.");
        }

        private Array CreateTypedArray(IEnumerable enumerable, Type arrayType)
        {
            var elementType = arrayType.GetElementType()
                ?? throw new InvalidOperationException("Cannot determine the element type of the target array.");

            var convertedValues = enumerable.Cast<object>()
                .Select(item => ConvertValue(item, elementType))
                .ToArray();

            var typedArray = Array.CreateInstance(elementType, convertedValues.Length);
            Array.Copy(convertedValues, typedArray, convertedValues.Length);
            return typedArray;
        }

        private static object ConvertStandardType(object value, Type targetType)
        {
            try
            {
                return Convert.ChangeType(value, targetType);
            }
            catch (Exception ex) when (ex is InvalidCastException || ex is FormatException)
            {
                throw new InvalidCastException($"Value '{value}' cannot be converted to type '{targetType.Name}'.", ex);
            }
        }

        private static object? ConvertJsonElement(JsonElement jsonElement, Type targetType)
        {
            return targetType switch
            {
                Type t when t == typeof(string) => jsonElement.GetString(),
                Type t when t == typeof(int) => jsonElement.TryGetInt32(out var intResult) ? intResult : throw new InvalidCastException($"Cannot convert '{jsonElement}' to int."),
                Type t when t == typeof(long) => jsonElement.TryGetInt64(out var longResult) ? longResult : throw new InvalidCastException($"Cannot convert '{jsonElement}' to long."),
                Type t when t == typeof(bool) => jsonElement.ValueKind switch
                {
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    _ => throw new InvalidCastException($"Cannot convert '{jsonElement}' to bool. Value is not a boolean.")
                },
                Type t when t == typeof(double) => jsonElement.TryGetDouble(out var doubleResult) ? doubleResult : throw new InvalidCastException($"Cannot convert '{jsonElement}' to double."),
                Type t when t == typeof(DateTime) => jsonElement.TryGetDateTime(out var dateResult) ? dateResult : throw new InvalidCastException($"Cannot convert '{jsonElement}' to DateTime."),
                _ => throw new InvalidCastException($"JsonElement type '{jsonElement.ValueKind}' cannot be converted to '{targetType.Name}'.")
            };
        }

        public IEnumerable<PropertyInfo> GetFilterableProperties<T>()
        {
            return typeof(T).GetProperties()
                .Where(p => IsFilterableType(p.PropertyType));
        }

        public IEnumerable<QueryOperator> GetValidOperatorsForProperty(PropertyInfo property)
        {
            ArgumentNullException.ThrowIfNull(property);

            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            return QueryOperator.All.Where(op => IsOperatorValidForType(op, type));
        }

        private static bool IsFilterableType(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            return underlyingType.IsPrimitive
                || underlyingType == typeof(string)
                || underlyingType == typeof(DateTime)
                || underlyingType == typeof(decimal)
                || underlyingType == typeof(Guid)
                || underlyingType.IsEnum;
        }

        private bool IsOperatorValidForType(QueryOperator op, Type propertyType)
        {
            return op switch
            {
                var o when o.IsNullOperation => true,
                var o when o.IsTextOperation => propertyType == typeof(string),
                var o when o.IsColumnOperation => true,
                var o when o.IsLogicalOperation => true,
                var o when (o.Value == "_eq" || o.Value == "_neq") => true,
                var o when (o.Value == "_gt" || o.Value == "_lt" || o.Value == "_gte" || o.Value == "_lte") => IsComparableType(propertyType),
                var o when (o.Value == "_in" || o.Value == "_nin") => true,
                _ => false,
            };
        }

        private static bool IsComparableType(Type type)
        {
            var comparableTypes = new HashSet<Type>
            {
                typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal),
                typeof(DateTime), typeof(TimeSpan), typeof(char), typeof(byte),
                typeof(sbyte), typeof(short), typeof(ushort), typeof(uint), typeof(ulong)
            };

            return comparableTypes.Contains(type) || type.IsEnum;
        }
    }
}
