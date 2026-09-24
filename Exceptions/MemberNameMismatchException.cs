using System;

namespace QueryBuilder.Exceptions
{
    public class MemberNameMismatchException : Exception
    {
        public string Member { get; }

        public MemberNameMismatchException(string member) : base(member)
        {
            Member = member;
        }
    }
}
