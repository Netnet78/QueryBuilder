using System;

namespace QueryBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    internal class OperatorAttribute : Attribute
    {
        private readonly string _value;

        public OperatorAttribute(string value)
        {
            _value = value;
        }

        public string Text => _value;
    }
}
