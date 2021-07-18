namespace FlatFiles
{
    internal class GenericExecutionContext : IExecutionContext
    {
        public GenericExecutionContext(ISchema? schema, IOptions options)
        {
            Schema = schema;
            Options = options;
        }

        public ISchema? Schema { get; }

        public IOptions Options { get; }
    }
}
