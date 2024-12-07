using QueryMatrixDemo.Core.Core.Models;
using QueryMatrixDemo.Core.Core.Operators;

namespace QueryMatrixDemo.Core.Core.Builders;

public interface IQueryMatrixFluentBuilder
{
    IQueryMatrixFluentBuilder WithLogicalOperator(QueryOperator op);
    IQueryMatrixFluentBuilder AddCondition(string field, QueryOperator op, object value);
    IQueryMatrixFluentBuilder AddCondition(string field, QueryOperator op, string pattern);
    IQueryMatrixFluentBuilder AddCondition(string field, QueryOperator op, IEnumerable<object> values);
    IQueryMatrixFluentBuilder AddNestedMatrix(QueryMatrix nestedMatrix);
    QueryMatrix Build();
}