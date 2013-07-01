using System;
using System.Collections.Generic;

namespace Aitako.DSL.Components
{
    public class Command
    {
        private readonly DomainObjectGeneratorBase _DomainObjectGeneratorBase;

        private Command(string name, DomainObjectGeneratorBase parentDomainObjectGeneratorBase)
        {
            Parameters = new List<Parameter> {Parameter.Create<Guid>(Make.ID_FIELD_NAME)};
            Name = name;
            _DomainObjectGeneratorBase = parentDomainObjectGeneratorBase;
        }

        public List<Parameter> Parameters { get; set; }

        public string Name { get; set; }

        internal static Command Create(string commandName, DomainObjectGeneratorBase parentDomainObject)
        {
            return new Command(commandName, parentDomainObject);
        }

        public Command WithParameter<T>(string parameterName)
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