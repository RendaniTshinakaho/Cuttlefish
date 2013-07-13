using System;
using System.Collections.Generic;

namespace Aitako.DSL.Components
{
    public class Event
    {
        private readonly DomainObjectGeneratorBase _DomainObjectGeneratorBase;

        private Event(string name, DomainObjectGeneratorBase parentDomainObjectGeneratorBase)
        {
            Parameters = new List<Parameter> {Parameter.Create<Guid>(Make.ID_FIELD_NAME)};
            Name = name;
            _DomainObjectGeneratorBase = parentDomainObjectGeneratorBase;
        }

        public List<Parameter> Parameters { get; set; }

        public string Name { get; set; }

        internal static Event Create(string eventName, DomainObjectGeneratorBase parentDomainObject)
        {
            return new Event(eventName, parentDomainObject);
        }

        public Event WithParameter<T>(string parameterName)
        {
            Parameters.Add(Parameter.Create<T>(parameterName));

            return this;
        }

        public DomainObjectGeneratorBase And()
        {
            return _DomainObjectGeneratorBase;
        }

        public DomainObjectGeneratorBase Done()
        {
            return _DomainObjectGeneratorBase;
        }
    }
}