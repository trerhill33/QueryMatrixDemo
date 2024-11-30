using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryMatrixDemo.Core.Helpers
{
    internal static class ExpressionHelper
    {
        // Cached MethodInfo for string.Contains and string.ToLowerInvariant
        private static readonly MethodInfo ContainsMethod = typeof(string).GetMethod("Contains", [typeof(string)])!;
        private static readonly MethodInfo ToLowerInvariantMethod = typeof(string).GetMethod("ToLowerInvariant")!;

        // Cached MethodInfo for Enumerable.Contains
        private static readonly MethodInfo EnumerableContainsMethod = typeof(Enumerable).GetMethods()
            .First(m => m.Name == "Contains" && m.GetParameters().Length == 2);

        public static Expression BuildStringComparisonExpression(Expression property, Expression pattern, StringComparison comparison)
        {
            // Default to simple string.Contains if no specific StringComparison is needed
            if (comparison == StringComparison.Ordinal || comparison == StringComparison.InvariantCulture)
            {
                return Expression.Call(property, ContainsMethod, pattern);
            }

            // Handle case-insensitive and other comparisons explicitly
            if (comparison == StringComparison.OrdinalIgnoreCase || comparison == StringComparison.InvariantCultureIgnoreCase)
            {
                // Use ToLowerInvariant for case-insensitive Contains
                var propertyLower = Expression.Call(property, ToLowerInvariantMethod);
                var patternLower = Expression.Call(pattern, ToLowerInvariantMethod);

                return Expression.Call(propertyLower, ContainsMethod, patternLower);
            }

            // Default fallback if unsupported
            throw new NotSupportedException($"The comparison type {comparison} is not supported.");
        }

        public static Expression BuildInExpression(Expression property, Expression values)
        {
            if (values is ConstantExpression constantExp && constantExp.Value is IEnumerable enumerable)
            {
                // Use cached Enumerable.Contains MethodInfo and make it generic
                var genericContainsMethod = EnumerableContainsMethod.MakeGenericMethod(property.Type);

                return Expression.Call(null, genericContainsMethod, Expression.Constant(enumerable), property);
            }

            throw new NotSupportedException("Invalid values for IN operator");
        }
    }
}
