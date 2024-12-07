using QueryMatrixDemo.Core.Core.Models;

namespace QueryMatrixDemo.Core.Core.Serialization;

public interface IQueryMatriSerializer
{
     string ToJson(QueryMatrix matrix, int maxDepth = 10);
     QueryMatrix FromJson(string json);
}
