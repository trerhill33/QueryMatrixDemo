using QueryMatrixDemo.Core.Core.Models;
using System.Text.Json;

namespace QueryMatrixDemo.Core.Core.Serialization;

/// <summary>
/// Implements serialization and deserialization of QueryMatrix objects to and from JSON.
/// </summary>
public class QueryMatriSerializer
{
    public static string ToJson(QueryMatrix matrix, int maxDepth = 10)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        // Implement serialization logic using System.Text.Json
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            MaxDepth = maxDepth
        };
        return JsonSerializer.Serialize(matrix, options);
    }

    public static QueryMatrix FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("JSON string cannot be null or whitespace.", nameof(json));
        // Implement deserialization logic
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            MaxDepth = 10
        };
        return JsonSerializer.Deserialize<QueryMatrix>(json, options) ?? throw new JsonException("Deserialization resulted in null.");
    }
}
