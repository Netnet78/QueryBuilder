using QueryBuilder.Attributes;

namespace QueryBuilder.Enums
{
    public enum Operator
    {
        [Operator("Contains")] Like,
        [Operator("Contains")] NotLike,
        [Operator("Equals")] Eq,
        [Operator("NotEquals")] Neq,
        [Operator("StartsWith")] Starts,
        [Operator("EndsWith")] Ends,
        [Operator("GreaterOrEquals")] Gte,
        [Operator("LesserOrEquals")] Lte,
        [Operator("GreaterThan")] Gt,
        [Operator("LesserThan")] Lt,
        [Operator("Contains")] In,
        [Operator("Contains")] NotIn,
        [Operator("IsNull")]IsNull,
        [Operator("IsNotNull")]IsNotNull,
    }
}
