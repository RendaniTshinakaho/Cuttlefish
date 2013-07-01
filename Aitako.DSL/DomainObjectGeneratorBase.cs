using System;
using System.Collections.Generic;
using System.Linq;
using Aitako.DSL.Components;
using Microsoft.VisualStudio.TextTemplating;

namespace Aitako.DSL
{
    public class DomainObjectGeneratorBase
    {
        static DomainObjectGeneratorBase()
        {
            All = new List<DomainObjectGeneratorBase>();
        }

        protected DomainObjectGeneratorBase(string name)
        {
            Name = name;
            Fields = new List<Field>();
            Commands = new List<Command>();
            Events = new List<Event>();
            All.Add(this);
        }

        public static List<DomainObjectGeneratorBase> All { get; private set; }

        public string Name { get; private set; }
        public string Parent { get; protected set; }
        public List<Field> Fields { get; private set; }
        public List<Command> Commands { get; private set; }
        public List<Event> Events { get; private set; }


        public static string Execute()
        {
            var template = Activator.CreateInstance<AggregateGenerator>();
            var session = new TextTemplatingSession();
            template.Session = session;
            //template.Initialize();
            string templateText = template.TransformText();

            Console.WriteLine(templateText);
            return templateText;
        }

        public DomainObjectGeneratorBase WithField<T>(string fieldName)
        {
            Fields.Add(Field.Create<T>(fieldName));

            return this;
        }

        public DomainObjectGeneratorBase ChildOf(string parent)
        {
            Parent = parent;
            return this;
        }

        public Command ThatCan(string commandName)
        {
            Command command = Command.Create(commandName, this);
            Commands.Add(command);

            return command;
        }

        public Event ThatRaises(string eventName)
        {
            Event @event = Event.Create(eventName, this);
            Events.Add(@event);

            return @event;
        }

        public DomainObjectGeneratorBase Done()
        {
            return this;
        }

        public DomainObjectGeneratorBase And()
        {
            return this;
        }

        public DomainObjectGeneratorBase ThatHasACreateTimestamp()
        {
            WithField<DateTime>("CreatedAt");
            return this;
        }

        public DomainObjectGeneratorBase ThatHasALastChangedTimestamp()
        {
            WithField<DateTime>("LastChanged");
            return this;
        }

        public DomainObjectGeneratorBase ThatCanBeHidden()
        {
            const string attributeName = "IsVisible";
            if (Fields.All(i => i.Name != attributeName))
            {
                WithField<bool>(attributeName);
            }

            ThatCan(string.Format("Hide{0}", Name)).WithParameter<Guid>("Id");
            ThatRaises(string.Format("{0}Hidden", Name)).WithParameter<Guid>("Id");

            return this;
        }

        public DomainObjectGeneratorBase ThatCanBeMadeVisible()
        {
            const string attributeName = "IsVisible";
            if (Fields.All(i => i.Name != attributeName))
            {
                WithField<bool>(attributeName);
            }

            ThatCan(string.Format("Make{0}Visible", Name)).WithParameter<Guid>("Id");
            ThatRaises(string.Format("{0}MadeVisible", Name)).WithParameter<Guid>("Id");

            return this;
        }
    }
}