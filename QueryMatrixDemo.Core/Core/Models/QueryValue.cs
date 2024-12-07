namespace QueryMatrixDemo.Core.Core.Models;

/// <summary>
/// Encapsulates the value(s) involved in a QueryCondition, supporting single values, arrays, column references, patterns, and null values, enabling versatile condition definitions.
/// </summary>
public sealed record QueryValue(object? Value, QueryValueType Type)
{
    public static QueryValue Single(object value) => new(value, QueryValueType.Single);
    public static QueryValue Array(IEnumerable<object> values) => new(values, QueryValueType.Array);
    public static QueryValue Column(string columnName) => new(columnName, QueryValueType.Column);
    public static QueryValue Pattern(string pattern) => new(pattern, QueryValueType.Pattern);
    public static QueryValue Null() => new(null, QueryValueType.Null);
}
