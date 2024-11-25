using Newtonsoft.Json.Linq;
using QueryMatrixDemo.Core.Models;

namespace QueryMatrixDemo.Core.Serialization;

public static class QueryMatrixSerializer
{
    public static string ToJson(QueryMatrix matrix, int maxDepth = 10)
    {
        if (maxDepth <= 0)
            throw new InvalidOperationException("Max depth exceeded while serializing QueryMatrix.");

        var obj = new JObject();

        if (matrix.Conditions.Any() || matrix.NestedMatrices.Any())
        {
            var array = new JArray();

            foreach (var condition in matrix.Conditions)
            {
                var condObj = new JObject
                {
                    [condition.Field] = new JObject
                    {
                        [condition.Operator.Value] = condition.Operator.IsColumnOperation
                            ? new JArray { condition.CompareToColumn }
                            : JToken.FromObject(condition.Value.Value)
                    }
                };
                array.Add(condObj);
            }

            foreach (var nested in matrix.NestedMatrices)
            {
                array.Add(JObject.Parse(ToJson(nested, maxDepth - 1)));
            }

            obj[matrix.LogicalOperator.Value] = array;
        }

        return obj.ToString();
    }
}
