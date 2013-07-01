using System.Linq;
using System.Text;

namespace Aitako.DSL.Components
{
    public class Aggregate : DomainObjectGeneratorBase
    {
        private Aggregate(string name)
            : base(name + "Aggregate")
        {
        }

        public static Aggregate AggregateNamed(string name)
        {
            return new Aggregate(name);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append("\n\n#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#\n\n");
            builder.AppendFormat("An aggregate named \"{0}\" has:\n", Name);


            foreach (Field field in Fields)
            {
                builder.AppendFormat("\n    + {0}", field.Name);
            }

            builder.Append("\n\nAnd can: \n");
            foreach (Command command in Commands)
            {
                builder.AppendFormat("\n    ! {0} with ", command.Name);
                foreach (Parameter parameter in command.Parameters)
                {
                    builder.AppendFormat("[{0}]", parameter.Name);
                    if (command.Parameters.Last() != parameter)
                    {
                        builder.Append(", ");
                    }
                }
            }

            builder.Append("\n\nAnd reacts when: \n");
            foreach (Event @event in Events)
            {
                builder.AppendFormat("\n    ? {0} with ", @event.Name);
                foreach (Parameter parameter in @event.Parameters)
                {
                    builder.AppendFormat("[{0}]", parameter.Name);
                    if (@event.Parameters.Last() != parameter)
                    {
                        builder.Append(", ");
                    }
                }
            }

            return builder.ToString();
        }
    }
}