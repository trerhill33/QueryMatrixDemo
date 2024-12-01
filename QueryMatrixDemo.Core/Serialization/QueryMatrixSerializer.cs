using Newtonsoft.Json.Linq;
using QueryMatrixDemo.Core.Models;

namespace QueryMatrixDemo.Core.Serialization
{
    public static class QueryMatrixSerializer
    {
        /// <summary>
        /// Serializes the provided <see cref="QueryMatrix"/> into a JSON string.
        /// </summary>
        /// <param name="matrix">The <see cref="QueryMatrix"/> to serialize.</param>
        /// <param name="maxDepth">The maximum depth for serialization to prevent stack overflow.</param>
        /// <returns>A JSON string representation of the <see cref="QueryMatrix"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="matrix"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="maxDepth"/> is exceeded.</exception>
        public static string ToJson(QueryMatrix matrix, int maxDepth = 10)
        {
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));

            var jObject = SerializeMatrix(matrix, maxDepth, 0);
            return jObject.ToString();
        }

        private static JObject SerializeMatrix(QueryMatrix matrix, int maxDepth, int currentDepth)
        {
            if (currentDepth >= maxDepth)
                throw new InvalidOperationException("Max depth exceeded while serializing QueryMatrix.");

            var obj = new JObject();

            if (matrix.Conditions.Any() || matrix.NestedMatrices.Count != 0)
            {
                var array = new JArray();

                foreach (var condition in matrix.Conditions)
                {
                    var conditionValue = condition.Operator.IsColumnOperation
                        ? new JArray(string.IsNullOrEmpty(condition.CompareToColumn) ? JValue.CreateNull() : condition.CompareToColumn)
                        : JToken.FromObject(condition.Value.Value);

                    var condObj = new JObject
                    {
                        [condition.Field] = new JObject
                        {
                            [condition.Operator.Value] = conditionValue
                        }
                    };
                    array.Add(condObj);
                }

                foreach (var nested in matrix.NestedMatrices)
                {
                    var nestedObj = SerializeMatrix(nested, maxDepth, currentDepth + 1);
                    array.Add(nestedObj);
                }

                obj[matrix.LogicalOperator.Value] = array;
            }

            return obj;
        }
    }
}
