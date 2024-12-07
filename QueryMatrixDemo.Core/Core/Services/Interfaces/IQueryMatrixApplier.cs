using QueryMatrixDemo.Core.Core.Models;

namespace QueryMatrixDemo.Core.Core.Services.Interfaces;

public interface IQueryMatrixApplier
{
    IQueryable<T> ApplyMatrix<T>(IQueryable<T> query, QueryMatrix matrix);
}
