using QueryMatrixDemo.Core.Core.Operators.Strategies.Interfaces;

namespace QueryMatrixDemo.Core.Core.Operators.Providers;

/// <summary>
/// Manages collections of value-based and column-based operator strategies, selecting the appropriate strategy based on the provided QueryOperator.
/// </summary>
/// <param name="valueStrategies"></param>
/// <param name="columnStrategies"></param>
public class OperatorStrategyProvider(IEnumerable<IValueOperatorStrategy> valueStrategies,
                                IEnumerable<IColumnOperatorStrategy> columnStrategies) : IOperatorStrategyProvider
{
    private readonly IEnumerable<IValueOperatorStrategy> _valueStrategies = valueStrategies 
        ?? throw new ArgumentNullException(nameof(valueStrategies));
    private readonly IEnumerable<IColumnOperatorStrategy> _columnStrategies = columnStrategies 
        ?? throw new ArgumentNullException(nameof(columnStrategies));

    public IValueOperatorStrategy GetValueStrategy(QueryOperator op)
    {
        var strategy = _valueStrategies.FirstOrDefault(s => s.CanHandle(op));
        return strategy ?? throw new NotSupportedException($"No value operator strategy found for operator '{op.Value}'.");
    }

    public IColumnOperatorStrategy GetColumnStrategy(QueryOperator op)
    {
        var strategy = _columnStrategies.FirstOrDefault(s => s.CanHandle(op));
        return strategy ?? throw new NotSupportedException($"No column operator strategy found for operator '{op.Value}'.");
    }
}
