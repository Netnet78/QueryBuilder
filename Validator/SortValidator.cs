using System.Collections.Generic;
using QueryBuilder.Exceptions;
using QueryBuilder.Models;

namespace QueryBuilder.Validator
{
    public class SortOrderValidator<T> : MemberNameValidator<T>
    {
        public bool Validate(SortOrder[] sortOrders, out IEnumerable<MemberNameMismatchException> errors)
        {
            var mismatches = new List<MemberNameMismatchException>();
            foreach (var sortOrder in sortOrders)
            {
                if (!IsValidMember(sortOrder.Member))
                    mismatches.Add(new MemberNameMismatchException(sortOrder.Member));
            }

            errors = mismatches;
            return mismatches.Count == 0;
        }
    }
}
