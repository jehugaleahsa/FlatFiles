namespace FlatFiles
{
    internal sealed class FixedLengthExecutionContext : IFixedLengthExecutionContext
    {
        public FixedLengthSchema Schema { get; set; }

        public FixedLengthOptions Options { get; set; }

        ISchema IExecutionContext.Schema => Schema;

        IOptions IExecutionContext.Options => Options;
    }
}
