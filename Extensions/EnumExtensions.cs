using System;
using QueryBuilder.Attributes;

namespace QueryBuilder.Extensions
{
    internal static class EnumExtensions
    {
        public static string GetOperator(this Enum enumValue)
        {
            var field = enumValue.GetType().GetField(enumValue.ToString());
            if (field == null)
                return enumValue.ToString();

            var attrs = field.GetCustomAttributes(typeof(OperatorAttribute), false);
            if (attrs == null || attrs.Length == 0)
                return string.Empty;

            var attribute = (OperatorAttribute)attrs[0];
            return attribute.Text ?? string.Empty;
        }
    }
}
