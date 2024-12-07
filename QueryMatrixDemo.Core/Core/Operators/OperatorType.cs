namespace QueryMatrixDemo.Core.Core.Operators;

/// <summary>
/// Enumerates the different types of operators used in query conditions, categorizing them into comparison, text, null checks, column comparisons, and logical operators.
/// </summary>
public enum OperatorType
{
    Comparison,
    Text,
    Null,
    ColumnComparison,
    Logical
}
