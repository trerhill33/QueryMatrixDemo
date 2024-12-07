using QueryMatrixDemo.Core.Core.Operators.Strategies.Interfaces;

namespace QueryMatrixDemo.Core.Core.Operators.Providers;

public interface IOperatorStrategyProvider
{
    IValueOperatorStrategy GetValueStrategy(QueryOperator op);
    IColumnOperatorStrategy GetColumnStrategy(QueryOperator op);
}
