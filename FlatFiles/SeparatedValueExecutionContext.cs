namespace FlatFiles
{
    internal sealed class SeparatedValueExecutionContext : ISeparatedValueExecutionContext
    {
        public SeparatedValueExecutionContext(SeparatedValueSchema? schema, SeparatedValueOptions options)
        {
            Schema = schema;
            Options = options;
        }

        public SeparatedValueSchema? Schema { get; }

        public SeparatedValueOptions Options { get; }

        ISchema? IExecutionContext.Schema => Schema;

        IOptions IExecutionContext.Options => Options;
    }
}
