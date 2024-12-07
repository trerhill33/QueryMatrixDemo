using System.Collections;
using System.Text.Json;
using QueryMatrixDemo.Core.Core.Services.Interfaces;

namespace QueryMatrixDemo.Core.Core.Services;

/// <summary>
/// Handles the conversion of input values to the target property types, ensuring type compatibility when building expressions, including handling enums, arrays, and nullable types.
/// </summary>
public class ValueConverter : IValueConverter
{
    public object? ConvertValue(object? value, Type targetType)
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
}
