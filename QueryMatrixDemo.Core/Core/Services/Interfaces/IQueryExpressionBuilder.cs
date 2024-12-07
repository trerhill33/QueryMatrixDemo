using QueryMatrixDemo.Core.Core.Models;
using System.Linq.Expressions;

namespace QueryMatrixDemo.Core.Core.Services.Interfaces;

public interface IQueryExpressionBuilder
{
    Expression<Func<T, bool>> BuildExpression<T>(QueryMatrix matrix);
}