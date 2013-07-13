namespace Aitako.DSL.Components
{
    public static class Make
    {
        public const string ID_FIELD_NAME = "AggregateIdentity";
        public static string BoundedContextName { get; private set; }
        public static string BaseNamespace { get; private set; }

        public static void InTheBoundedContextOf(string contextName)
        {
            BoundedContextName = contextName;
        }

        public static void WithNamespace(string namespaceName)
        {
            BaseNamespace = namespaceName;
        }
    }
}