using System;
using System.Reflection;

namespace QueryBuilder.Validator
{
    public class MemberNameValidator<T>
    {
        public bool IsValidMember(string memberPath)
        {
            if (string.IsNullOrWhiteSpace(memberPath))
                return false;

            Type currentType = typeof(T);
            foreach (var part in memberPath.Split('.'))
            {
                var prop = currentType.GetProperty(
                    part, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (prop != null)
                {
                    currentType = prop.PropertyType;
                    continue;
                }

                var field = currentType.GetField(
                    part, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (field != null)
                {
                    currentType = field.FieldType;
                    continue;
                }

                return false;
            }

            return true;
        }
    }
}
