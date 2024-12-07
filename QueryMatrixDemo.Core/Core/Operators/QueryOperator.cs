﻿namespace QueryMatrixDemo.Core.Core.Operators;

/// <summary>
/// Defines various operators used in query conditions, including standard comparison, text matching, null checks, column comparisons, and logical operators, each associated with a value, description, and type.
/// </summary>
public sealed record QueryOperator(string Value, string Description, OperatorType Type = OperatorType.Comparison)
{
    public bool IsColumnOperation => Type == OperatorType.ColumnComparison;
    public bool IsTextOperation => Type == OperatorType.Text;
    public bool IsNullOperation => Type == OperatorType.Null;
    public bool IsLogicalOperation => Type == OperatorType.Logical;

    #region Standard Comparison Operators
    public static readonly QueryOperator Equal = new("_eq", "equals");
    public static readonly QueryOperator NotEqual = new("_neq", "not equals");
    public static readonly QueryOperator GreaterThan = new("_gt", "greater than");
    public static readonly QueryOperator LessThan = new("_lt", "less than");
    public static readonly QueryOperator GreaterThanOrEqual = new("_gte", "greater than or equal");
    public static readonly QueryOperator LessThanOrEqual = new("_lte", "less than or equal");
    public static readonly QueryOperator In = new("_in", "in array");
    public static readonly QueryOperator NotIn = new("_nin", "not in array");
    #endregion

    #region Text Matching Operators
    public static readonly QueryOperator Like = new("_like", "pattern match", OperatorType.Text);
    public static readonly QueryOperator ILike = new("_ilike", "case insensitive pattern match", OperatorType.Text);
    public static readonly QueryOperator NotLike = new("_nlike", "pattern not match", OperatorType.Text);
    public static readonly QueryOperator Similar = new("_similar", "similar to", OperatorType.Text);
    public static readonly QueryOperator NotSimilar = new("_nsimilar", "not similar to", OperatorType.Text);
    public static readonly QueryOperator Regex = new("_regex", "regex match", OperatorType.Text);
    public static readonly QueryOperator IRegex = new("_iregex", "case insensitive regex match", OperatorType.Text);
    #endregion

    #region Null Check Operator
    public static readonly QueryOperator IsNull = new("_is_null", "is null check", OperatorType.Null);
    #endregion

    #region Column Comparison Operators
    public static readonly QueryOperator ColumnEqual = new("_ceq", "column equals", OperatorType.ColumnComparison);
    public static readonly QueryOperator ColumnNotEqual = new("_cne", "column not equals", OperatorType.ColumnComparison);
    public static readonly QueryOperator ColumnGreaterThan = new("_cgt", "column greater than", OperatorType.ColumnComparison);
    public static readonly QueryOperator ColumnLessThan = new("_clt", "column less than", OperatorType.ColumnComparison);
    public static readonly QueryOperator ColumnGreaterThanOrEqual = new("_cgte", "column greater than or equal", OperatorType.ColumnComparison);
    public static readonly QueryOperator ColumnLessThanOrEqual = new("_clte", "column less than or equal", OperatorType.ColumnComparison);
    public static readonly QueryOperator ColumnLike = new("_clike", "column pattern match", OperatorType.ColumnComparison);
    public static readonly QueryOperator ColumnILike = new("_cilike", "column case insensitive pattern match", OperatorType.ColumnComparison);
    #endregion

    #region Logical Operators
    public static readonly QueryOperator And = new("_and", "logical and", OperatorType.Logical);
    public static readonly QueryOperator Or = new("_or", "logical or", OperatorType.Logical);
    public static readonly QueryOperator Not = new("_not", "logical not", OperatorType.Logical);
    #endregion

    public static readonly IReadOnlyCollection<QueryOperator> All =
    [
        Equal, NotEqual, GreaterThan, LessThan, GreaterThanOrEqual, LessThanOrEqual, In, NotIn,
        Like, ILike, NotLike, Similar, NotSimilar, Regex, IRegex,
        IsNull,
        ColumnEqual, ColumnNotEqual, ColumnGreaterThan, ColumnLessThan,
        ColumnGreaterThanOrEqual, ColumnLessThanOrEqual, ColumnLike, ColumnILike,
        And, Or, Not
    ];

    public static QueryOperator FromString(string value) =>
        All.FirstOrDefault(op => op.Value == value)
        ?? throw new ArgumentException($"Unsupported operator: {value}", nameof(value));
}