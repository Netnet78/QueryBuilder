using System;
using QueryBuilder.Enums;
using System.Linq.Expressions;
using QueryBuilder.Expressions;

namespace QueryBuilder.Models
{
    public class SortKey<T>
    {
        public Expression<Func<T, object>> KeySelector { get; set; }
        public SortDirection Direction { get; set; }

        public SortKey(Expression<Func<T, object>> keySelector, SortDirection direction = SortDirection.Ascending)
        {
            KeySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
            Direction = direction;
        }

        public static SortKey<T> FromSortOrder(SortOrder sortOrder)
        {
            var keySelector = ExpressionMethods.CreateKeySelector<T>(sortOrder.Member);
            return new SortKey<T>(keySelector, sortOrder.Direction);
        }
    }
}
