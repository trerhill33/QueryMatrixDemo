namespace QueryMatrixDemo.Core.Core.Utilities;

/// <summary>
/// Provides static helper methods for validating method parameters, ensuring that inputs meet expected conditions (e.g., not null, not empty), promoting code reuse and consistency across the system.
/// </summary>
public static class ValidationHelper
{
    public static void EnsureNotNull(object obj, string paramName)
    {
        if (obj == null)
            throw new ArgumentNullException(paramName);
    }

    public static void EnsureNotNullOrWhiteSpace(string str, string paramName)
    {
        if (string.IsNullOrWhiteSpace(str))
            throw new ArgumentException("Value cannot be null or whitespace.", paramName);
    }
}
