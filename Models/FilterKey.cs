using System;
using QueryBuilder.Enums;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using QueryBuilder.Expressions;

namespace QueryBuilder.Models
{
    /// <summary>
    /// Represents a filter key that pairs a property selector with a string value or a set of string values and an
    /// operator for filtering.
    /// </summary>
    /// <remarks>Contains either a single Value or multiple Values. Use FromFilter to create an instance from
    /// a Filter. Constructors throw ArgumentNullException for a null selector or null value/values.</remarks>
    /// <typeparam name="T">The type of the object that contains the property selected by the KeySelector.</typeparam>
    public class FilterKey<T>
    {
        /// <summary>
        /// Gets or sets an expression that selects a key from an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>Used to obtain a key for grouping, ordering, or lookup. The expression should be
        /// side-effect free and may be translated by LINQ providers (for example, to SQL); avoid client-only methods or
        /// constructs that cannot be translated. The selector may return a single value or an anonymous/composite
        /// key.</remarks>
        public Expression<Func<T, object>> KeySelector { get; set; }
        /// <summary>
        /// Gets or sets the string value.
        /// </summary>
        /// <remarks>May be null</remarks>
        public string? Value { get; set; }
        /// <summary>
        /// Gets or sets the sequence of string values.
        /// </summary>
        /// <remarks>May be null.</remarks>
        public IEnumerable<string>? Values { get; set; }
        /// <summary>
        /// Gets or sets the operator.
        /// </summary>
        /// <remarks>Specifies the operation to apply, represented by an Operator enum value.</remarks>
        public Operator Operator { get; set; }

        /// <summary>
        /// Initializes a new FilterKey with a key selector, a value, and an operator.
        /// </summary>
        /// <param name="keySelector">Expression that selects the key from T.</param>
        /// <param name="value">Value to compare against the selected key.</param>
        /// <param name="operator">Comparison operator to apply; if omitted, the default Operator value is used.</param>
        /// <exception cref="ArgumentNullException">Thrown when keySelector or value is null.</exception>
        public FilterKey(Expression<Func<T, object>> keySelector, string value, Operator @operator = default)
        {
            KeySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Operator = @operator;
        }

        /// <summary>
        /// Initializes a new FilterKey that extracts a key from T using the provided selector and filters by the
        /// specified values and operator.
        /// </summary>
        /// <param name="keySelector">Expression that selects the key from an instance of T.</param>
        /// <param name="values">Sequence of string values to filter against.</param>
        /// <param name="operator">Operator to apply when comparing the selected key to the provided values.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="keySelector"/> or <paramref name="values"/> is null.</exception>
        public FilterKey(Expression<Func<T, object>> keySelector, IEnumerable<string> values, Operator @operator = default)
        {
            KeySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
            Values = values ?? throw new ArgumentNullException(nameof(values));
            Operator = @operator;
        }

        /// <summary>
        /// Creates a FilterKey<T> from a key selector and a Filter, using the filter's Values when present or its
        /// single Value otherwise.
        /// </summary>
        /// <remarks>If filter.Values contains any elements, those values are used; otherwise filter.Value
        /// is used.</remarks>
        /// <param name="keySelector">Expression that selects the key from T.</param>
        /// <param name="filter">Filter that provides either multiple Values or a single Value and the Operator to apply.</param>
        /// <returns>A FilterKey<T> initialized with the selected key, the filter's value(s), and its operator.</returns>
        public static FilterKey<T> FromFilter(
            Expression<Func<T, object>> keySelector, Filter filter)
        {
            if (filter.Values is not null && filter.Values.Any())
                return new FilterKey<T>(keySelector, filter.Values, filter.Operator);
            else if (filter.Value is not null)
                return new FilterKey<T>(keySelector, filter.Value, filter.Operator);
            else throw new Exception(nameof(filter.Value) + $" cannot be null or empty. You must initialize and set either `{nameof(filter.Value)}` or `{nameof(filter.Values)}`");
        }

        /// <summary>
        /// Creates a FilterKey<T> from a Filter by building a key selector from the Filter's Member and deriving a
        /// value from Filter.Values or Filter.Value.
        /// </summary>
        /// <remarks>If Filter.Values is non-empty, values are joined with commas; otherwise Filter.Value
        /// is used. The key selector is produced by ExpressionMethods.CreateKeySelector<T>(filter.Member).</remarks>
        /// <param name="filter">Filter containing Member, Values (preferred when present) or Value, and Operator used to construct the
        /// FilterKey.</param>
        /// <returns>A FilterKey<T> whose key selector is created from filter.Member, whose value is the comma-joined Values if
        /// present or the Value otherwise, and whose operator is filter.Operator.</returns>
        public static FilterKey<T> FromFilter(Filter filter)
        {
            var keySelector = ExpressionMethods.CreateKeySelector<T>(filter.Member);

            if (filter.Values is not null && filter.Values.Any())
                return new FilterKey<T>(keySelector, filter.Values, filter.Operator);
            else if (filter.Value is not null)
                return new FilterKey<T>(keySelector, filter.Value, filter.Operator);
            else throw new Exception(nameof(filter.Value) + $" cannot be null or empty. You must initialize and set either `{nameof(filter.Value)}` or `{nameof(filter.Values)}`");
        }
    }
}
