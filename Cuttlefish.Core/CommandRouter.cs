using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cuttlefish.Common;
using Cuttlefish.Common.Exceptions;
using Fasterflect;

namespace Cuttlefish
{
    /// <summary>
    ///     Routes a given command the the aggregate/service it should be run against. The plan is to abstract this so that commands can be fired against any aggregate.
    /// </summary>
    public class CommandRouter
    {
        private const string CommandHandlerMethodName = "On";

        private static readonly CommandRouter Router;

        private readonly Dictionary<string, MethodInfo> _commandDictionary;

        static CommandRouter()
        {
            Router = new CommandRouter();
        }

        private CommandRouter()
        {
            _commandDictionary = new Dictionary<string, MethodInfo>();

            IEnumerable<Assembly> assemblies =
                AppDomain.CurrentDomain.GetAssemblies()
                         .Where(i => i.FullName.ToLower().StartsWith(Core.Instance.NamespaceRoot.ToLower()));

            foreach (Assembly assembly in assemblies)
            {
                IEnumerable<Type> types =
                    assembly.Types()
                            .Where(
                                m =>
                                m.Methods().Any(i => i.Name == CommandHandlerMethodName && i.Parameters().Count == 1));

                foreach (Type type in types)
                {
                    Dictionary<Type, MethodInfo> methods = type.MethodsWith(BindingFlags.Public |
                                                                            BindingFlags.NonPublic |
                                                                            BindingFlags.Instance)
                                                               .Where(i => i.DeclaringType == type)
                                                               .Where(
                                                                   m =>
                                                                   m.Name == CommandHandlerMethodName &&
                                                                   m.GetParameters().Length == 1)
                                                               .ToDictionary(
                                                                   m => m.GetParameters().First().ParameterType, m => m);

                    foreach (var item in methods)
                    {
                        _commandDictionary.Add(item.Key.Name, item.Value);
                    }
                }
            }
        }

        public static void ExecuteCommand(ICommand command)
        {
            Router.ExecuteCommandInternal(command);
        }

        private void ExecuteCommandInternal(ICommand command)
        {
            string commandTypeName = command.GetType().Name;
            if (_commandDictionary.ContainsKey(commandTypeName))
            {
                MethodInfo methodInfo = _commandDictionary[commandTypeName];
                Type handlerType = methodInfo.DeclaringType;

                if (handlerType != null)
                {
                    if (handlerType.GetInterfaces().Any(i => i.Name == typeof (IService).Name))
                    {
                        object service = handlerType.CreateInstance();
                        service.CallMethod(CommandHandlerMethodName, command);
                    }
                    else
                    {
                        AggregateBase aggregateBase = null;
                        if (Core.Instance.Cache != null && Core.Instance.UseCaching)
                        {
                            aggregateBase =
                                (AggregateBase) Core.Instance.Cache.Fetch(command.AggregateIdentity, handlerType);
                            if (aggregateBase == null)
                            {
                                aggregateBase =
                                    (AggregateBase)
                                    handlerType.CreateInstance(
                                        Core.Instance.EventStore.GetEvents(command.AggregateIdentity));

                                Core.Instance.Cache.Cache(aggregateBase);
                            }
                        }
                        else
                        {
                            aggregateBase =
                                (AggregateBase)
                                handlerType.CreateInstance(
                                    Core.Instance.EventStore.GetEvents(command.AggregateIdentity));
                        }


                        aggregateBase.CallMethod(CommandHandlerMethodName, command);
                    }
                }
            }
            else
            {
                throw new NoHandlerFoundException(command.GetType());
            }
        }
    }
}