using System;

namespace Lotech.Data.Operations
{
    public class ExpressionParameter
    {
        public ExpressionParameter(string name, Type type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }

        public string Name { get; }

        public Type Type { get; }

        public object Value { get; }
    }
}
