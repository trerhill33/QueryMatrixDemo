using QueryMatrixDemo.Core.Core.Models;
using QueryMatrixDemo.Core.Core.Services.Interfaces;

namespace QueryMatrixDemo.Core.Core.Services;

/// <summary>
///Applies a QueryMatrix to an IQueryable<T> data source by utilizing the IQueryExpressionBuilder to generate the necessary LINQ expressions and filtering the data accordingly.
/// </summary>
/// <param name="expressionBuilder"></param>
public class QueryMatrixApplier(IQueryExpressionBuilder expressionBuilder) : IQueryMatrixApplier
{
    private readonly IQueryExpressionBuilder _expressionBuilder = expressionBuilder 
        ?? throw new ArgumentNullException(nameof(expressionBuilder));

    public IQueryable<T> ApplyMatrix<T>(IQueryable<T> query, QueryMatrix matrix)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(matrix);

        var expression = _expressionBuilder.BuildExpression<T>(matrix);
        return query.Where(expression);
    }
}
