using System;
using QueryBuilder.Enums;

namespace QueryBuilder.Models
{
    public class SortOrder
    {
        public string Member { get; set; }
        public SortDirection Direction { get; set; }

        public SortOrder(string member, SortDirection direction)
        {
            Member = member ?? throw new ArgumentNullException(nameof(member));
            Direction = direction;
        }
    }
}
