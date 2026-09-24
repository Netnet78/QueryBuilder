using QueryBuilder.Enums;
using QueryBuilder.Extensions;
using QueryBuilder.Models;
using QueryBuilder.Validator;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryBuilder.Expressions
{
    public static class ExpressionMethods
    {
        public static Expression<Func<T, object>> CreateKeySelector<T>(string memberPath)
        {
            if (string.IsNullOrWhiteSpace(memberPath))
                throw new ArgumentException("Member path cannot be null or empty.", nameof(memberPath));
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression memberExpression = parameter;
            // Handle nested properties e.g. "Address.City"
            foreach (var memberName in memberPath.Split('.'))
            {
                var prop = memberExpression.Type.GetProperty(
                    memberName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop != null)
                {
                    memberExpression = Expression.Property(memberExpression, prop);
                    continue;
                }
                var field = memberExpression.Type.GetField(
                    memberName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (field != null)
                {
                    memberExpression = Expression.Field(memberExpression, field);
                    continue;
                }
                throw new ArgumentException(
                    $"Member '{memberName}' was not found on type '{memberExpression.Type.Name}'.");
            }
            // Box value types to object to satisfy Func<T, object>
            var boxedExpression = Expression.Convert(memberExpression, typeof(object));
            return Expression.Lambda<Func<T, object>>(boxedExpression, parameter);
        }

        public static string GetPropertyName<T>(Expression<Func<T, object>> expression)
        {
            if (!(expression.Body is MemberExpression body))
            {
                UnaryExpression ubody = (UnaryExpression)expression.Body;
                body = (MemberExpression)ubody.Operand;
            }

            return body.Member.Name;
        }

        public static Expression CreateExpression<T>(
ParameterExpression param, FilterKey<T> filter)
        {
            var visitor = new ParameterUpdateVisitor(
                filter.KeySelector.Parameters.First(), param);
            var newEx = visitor.Visit(filter.KeySelector);

            var member = GetMemberExpression(newEx);
            var propertyType = ((PropertyInfo)member.Member).PropertyType;
            var valueExpression = GetValueExpression(member, filter);
            if (filter.Operator == Operator.Eq)
                return Expression.Equal(member, valueExpression);
            if (filter.Operator == Operator.Neq)
                return Expression.NotEqual(member, valueExpression);
            if (filter.Operator == Operator.Gte)
                return Expression.GreaterThanOrEqual(member, valueExpression);
            if (filter.Operator == Operator.Lte)
                return Expression.LessThanOrEqual(member, valueExpression);
            if (filter.Operator == Operator.Gt)
                return Expression.GreaterThan(member, valueExpression);
            if (filter.Operator == Operator.Lt)
                return Expression.LessThan(member, valueExpression);
            // String and collection operators
            Expression stringExpression = member;
            if (propertyType != typeof(string))
                stringExpression = Expression.Call(
                    member, nameof(member.ToString), Type.EmptyTypes);
            return filter.Operator switch
            {
                Operator.Like => Expression.Call(
                    stringExpression, filter.Operator.GetOperator(),
                    Type.EmptyTypes, valueExpression),
                Operator.NotLike => Expression.Not(Expression.Call(
                    stringExpression, filter.Operator.GetOperator(),
                    Type.EmptyTypes, valueExpression)),
                Operator.Starts => Expression.Call(
                    stringExpression, filter.Operator.GetOperator(),
                    Type.EmptyTypes, valueExpression),
                Operator.Ends => Expression.Call(
                    stringExpression, filter.Operator.GetOperator(),
                    Type.EmptyTypes, valueExpression),
                Operator.In => Expression.Call(
                    typeof(Enumerable), filter.Operator.GetOperator(),
                    [propertyType], valueExpression, member),
                Operator.NotIn => Expression.Not(Expression.Call(
                    typeof(Enumerable), filter.Operator.GetOperator(),
                    [propertyType], valueExpression, member)),
                _ => throw new NotSupportedException(),
            };
        }

        private static MemberExpression GetMemberExpression(Expression expression)
        {
            if (expression is MemberExpression mex)
                return mex;

            if (expression is UnaryExpression uex)
                return (MemberExpression)uex.Operand;

            return expression is LambdaExpression lex
                ? lex.Body is UnaryExpression luex
                    ? (MemberExpression)luex.Operand
                    : (MemberExpression)lex.Body
                : throw new NotSupportedException();
        }

        private static Expression GetValueExpression<T>(
    MemberExpression member, FilterKey<T> filter)
        {
            var propertyType = ((PropertyInfo)member.Member).PropertyType;

            // In/NotIn: split comma-separated string or enumerate into typed array
            if (filter.Operator == Operator.In || filter.Operator == Operator.NotIn)
            {
                var items = filter.Values is not null && filter.Values.Any() ?
                    filter.Values.ToArray() : 
                    filter.Value!.Split(',').Select(x => x.Trim()).ToArray();
                if (propertyType == typeof(string))
                    return Expression.Constant(items);
                var arr = Array.CreateInstance(propertyType, items.Length);
                var converter = TypeDescriptor.GetConverter(propertyType);
                for (var i = 0; i < items.Length; i++)
                    arr.SetValue(converter.ConvertFromString(
                        null, CultureInfo.CurrentCulture, items[i]), i);
                return Expression.Constant(arr);
            }

            // String operators: value stays as string
            if (propertyType == typeof(string) ||
                filter.Operator == Operator.Like ||
                filter.Operator == Operator.NotLike ||
                filter.Operator == Operator.Starts ||
                filter.Operator == Operator.Ends)
                return Expression.Constant(filter.Value);

            // Everything else: convert string to target type
            var propertyValue = TypeDescriptor.GetConverter(propertyType)
                .ConvertFromString(null, CultureInfo.CurrentCulture, filter.Value?.Trim() ?? string.Empty);
            return Expression.Convert(
                Expression.Constant(propertyValue), propertyType);
        }
    }
}
