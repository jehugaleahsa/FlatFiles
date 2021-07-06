namespace FlatFiles
{
    internal sealed class SeparatedValueExecutionContext : ISeparatedValueExecutionContext
    {
        public SeparatedValueSchema Schema { get; set; }

        public SeparatedValueOptions Options { get; set; }

        ISchema IExecutionContext.Schema => Schema;

        IOptions IExecutionContext.Options => Options;
    }
}
