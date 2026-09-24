using System;
using System.Collections.Generic;
using QueryBuilder.Enums;

namespace QueryBuilder.Models
{
    public class Filter
    {
        public string Member { get; set; }
        public string? Value { get; set; }
        public IEnumerable<string>? Values { get; set; }
        public Operator Operator { get; set; }

        public Filter(string member, string value, Operator @operator)
        {
            Member = member ?? throw new ArgumentNullException(nameof(member));
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Operator = @operator;
        }

        public Filter(string member, IEnumerable<string> values, Operator @operator)
        {
            Member = member ?? throw new ArgumentNullException(nameof(member));
            Values = values ?? throw new ArgumentNullException(nameof(values));
            Operator = @operator;
        }
    }
}
