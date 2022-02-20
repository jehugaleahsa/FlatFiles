namespace FlatFiles
{
    internal sealed class DelimitedExecutionContext : IDelimitedExecutionContext
    {
        public DelimitedExecutionContext(DelimitedSchema schema, DelimitedOptions options)
        {
            Schema = schema;
            Options = options;
        }

        public DelimitedSchema Schema { get; }

        public DelimitedOptions Options { get; }

        ISchema IExecutionContext.Schema => Schema;

        IOptions IExecutionContext.Options => Options;
    }
}
