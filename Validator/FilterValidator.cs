using System.Collections.Generic;
using QueryBuilder.Exceptions;
using QueryBuilder.Models;

namespace QueryBuilder.Validator
{
    public class FilterValidator<T> : MemberNameValidator<T>
    {
        public bool Validate(Filter[] filters, out IEnumerable<MemberNameMismatchException> errors)
        {
            var mismatches = new List<MemberNameMismatchException>();
            foreach (var filter in filters)
            {
                if (!IsValidMember(filter.Member))
                    mismatches.Add(new MemberNameMismatchException(filter.Member));
            }

            errors = mismatches;
            return mismatches.Count == 0;
        }
    }
}
