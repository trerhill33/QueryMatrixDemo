using QueryMatrixDemo.Core.Core.Models;
using QueryMatrixDemo.Core.Core.Services.Interfaces;

namespace QueryMatrixDemo.Core.Core.Services;

public class QueryMatrixProcessor(IQueryMatrixApplier applier) : IQueryMatrixProcessor
{
    private readonly IQueryMatrixApplier _applier = applier;

    public IQueryable<T> ApplyMatrix<T>(IQueryable<T> query, QueryMatrix matrix)
    {
        return _applier.ApplyMatrix(query, matrix);
    }
}
