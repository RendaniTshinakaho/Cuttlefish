using System;

namespace Aitako.DSL.Components
{
    public class Field
    {
        private Field(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; set; }
        public Type Type { get; set; }

        internal static Field Create<T>(string fieldName)
        {
            return new Field(fieldName, typeof (T));
        }
    }
}