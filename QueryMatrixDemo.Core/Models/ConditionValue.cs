namespace QueryMatrixDemo.Core.Models;

public sealed record ConditionValue(object? Value, ConditionValueType Type)
{
    public static ConditionValue Single(object value) => new(value, ConditionValueType.Single);
    public static ConditionValue Array(IEnumerable<object> values) => new(values, ConditionValueType.Array);
    public static ConditionValue Column(string columnName) => new(columnName, ConditionValueType.Column);
    public static ConditionValue Pattern(string pattern) => new(pattern, ConditionValueType.Pattern);
    public static ConditionValue Null() => new(null, ConditionValueType.Null);
}
