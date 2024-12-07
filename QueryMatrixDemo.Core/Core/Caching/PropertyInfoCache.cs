using System.Collections.Concurrent;
using System.Reflection;

namespace QueryMatrixDemo.Core.Core.Caching;

/// <summary>
/// Implements caching for PropertyInfo objects to enhance performance by reducing reflection overhead.
/// </summary>
public class PropertyInfoCache : IPropertyInfoCache
{
    // ConcurrentDictionary to ensure thread-safe access.
    private readonly ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>> _cache
        = new();

    /// <summary>
    /// Retrieves the PropertyInfo for the specified type and property name, utilizing caching to improve performance.
    /// </summary>
    public PropertyInfo GetProperty<T>(string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            throw new ArgumentException("Property name cannot be null or whitespace.", nameof(propertyName));

        var type = typeof(T);

        // Get or add the property dictionary for the type.
        var properties = _cache.GetOrAdd(type, t =>
            t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
             .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase));

        // Try to get the PropertyInfo from the dictionary.
        if (!properties.TryGetValue(propertyName, out var propertyInfo))
            throw new ArgumentException($"Property '{propertyName}' does not exist on type '{type.FullName}'.", nameof(propertyName));

        return propertyInfo;
    }
}
