using QueryBuilder.Enum;
using System.Linq.Expressions;

namespace QueryBuilder.Models
{
    public class FilterCondition<T>
    {
        public Expression<Func<T, object>> Property { get; private set; } = default!;
        public FilterOperator Operator { get; private set; }
        public object? Value { get; private set; }
        public IEnumerable<object>? Values { get; private set; }

        public FilterCondition(Expression<Func<T, object>> expression, FilterOperator op, object? value)
        {
            Property = expression;
            Operator = op;
            Value = value;
        }
        public FilterCondition(
            Expression<Func<T, object>> expression,
            FilterOperator op,
            IEnumerable<object>? values)
        {
            Property = expression;
            Operator = op;
            Values = values;
        }
    }
}
