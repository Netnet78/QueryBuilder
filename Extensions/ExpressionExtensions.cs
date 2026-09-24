using System;
using System.Collections.Generic;
using System.Linq;
using QueryBuilder.Enums;
using QueryBuilder.Expressions;
using QueryBuilder.Models;
using System.Linq.Expressions;

namespace QueryBuilder.Extensions
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(
                Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
                                                       IEnumerable<FilterKey<T>> filters)
        {
            if (!filters.Any())
                return expr1;

            var param = Expression.Parameter(typeof(T), "x");
            Expression? body = null;
            foreach (var filter in filters)
                body = body == null
                    ? ExpressionMethods.CreateExpression(param, filter)
                    : Expression.AndAlso(body, ExpressionMethods.CreateExpression(param, filter));

            return expr1.And(Expression.Lambda<Func<T, bool>>(body!, param));
        }

        public static IQueryable<T> FilterByDynamic<T>(this IQueryable<T> query, IEnumerable<FilterKey<T>> filterKeys)
        {
            if (!filterKeys.Any())
                return query;

            ParameterExpression param = Expression.Parameter(typeof(T));
            foreach (FilterKey<T> filterKey in filterKeys)
            {
                query = query.Provider.CreateQuery<T>()
            }
        }

        public static IQueryable<T> OrderByDynamic<T>(
    this IQueryable<T> query, IEnumerable<SortKey<T>> sortKeys)
        {
            if (!sortKeys.Any())
                return query;

            var param = Expression.Parameter(typeof(T));
            var first = true;
            foreach (var sortOrder in sortKeys)
            {
                var memberType = sortOrder.KeySelector.ReturnType;
                var orderBy = Expression.Call(typeof(Queryable),
                    sortOrder.Direction == SortDirection.Ascending
                        ? first ? nameof(Queryable.OrderBy)
                                : nameof(Queryable.ThenBy)
                        : first ? nameof(Queryable.OrderByDescending)
                                : nameof(Queryable.ThenByDescending),
                    new Type[] { typeof(T), memberType },
                    query.Expression,
                    Expression.Quote(sortOrder.KeySelector));
                query = query.Provider.CreateQuery<T>(orderBy);
                first = false;
            }
            return query;
        }
    }
}
