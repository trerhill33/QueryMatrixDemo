using System.Linq.Expressions;
using System.Reflection;
using QueryMatrixDemo.Core.Models;
using QueryMatrixDemo.Core.Operators;

namespace QueryMatrixDemo.Core.Interfaces;

public interface IQueryMatrixService
{
    IQueryable<T> ApplyMatrix<T>(IQueryable<T> query, QueryMatrix matrix);
    Expression<Func<T, bool>> BuildExpression<T>(QueryMatrix matrix);
    IEnumerable<PropertyInfo> GetFilterableProperties<T>();
    IEnumerable<QueryOperator> GetValidOperatorsForProperty(PropertyInfo property);
}