namespace QueryMatrixDemo.Core.Core.Models;

/// <summary>
/// Enumerates the different types of values that a QueryValue can represent, facilitating appropriate handling during expression building.
/// </summary>
public enum QueryValueType
{
    Single,
    Array,
    Column,
    Pattern,
    Null
}
