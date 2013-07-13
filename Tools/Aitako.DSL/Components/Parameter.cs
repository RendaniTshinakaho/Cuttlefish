using System;

namespace Aitako.DSL.Components
{
    public class Parameter
    {
        private Parameter(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; set; }
        public Type Type { get; set; }

        internal static Parameter Create<T>(string fieldName)
        {
            return new Parameter(fieldName, typeof (T));
        }
    }
}