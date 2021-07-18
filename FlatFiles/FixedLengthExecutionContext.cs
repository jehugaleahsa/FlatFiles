namespace FlatFiles
{
    internal sealed class FixedLengthExecutionContext : IFixedLengthExecutionContext
    {
        public FixedLengthExecutionContext(FixedLengthSchema schema, FixedLengthOptions options)
        {
            Schema = schema; // We guarantee Schema returns non-null before exposing interface
            Options = options;
        }

        public FixedLengthSchema Schema { get; set; }

        public FixedLengthOptions Options { get; }

        ISchema IExecutionContext.Schema => Schema;

        IOptions IExecutionContext.Options => Options;
    }
}
