namespace FlatFiles
{
    /// <summary>
    /// Holds information about the currently running process.
    /// </summary>
    public interface IExecutionContext
    {
        /// <summary>
        /// Gets the schema being used to process the records.
        /// </summary>
        ISchema Schema { get; }

        /// <summary>
        /// Gets the global options being used to process the records.
        /// </summary>
        IOptions Options { get; }
    }

    /// <summary>
    /// Holds information about the currently running process.
    /// </summary>
    public interface IFixedLengthExecutionContext : IExecutionContext
    {
        /// <summary>
        /// Gets the schema being used to process the records.
        /// </summary>
        new FixedLengthSchema Schema { get; }

        /// <summary>
        /// Gets the global options being used to process the records.
        /// </summary>
        new FixedLengthOptions Options { get; }
    }

    /// <summary>
    /// Holds information about the currently running process.
    /// </summary>
    public interface ISeparatedValueExecutionContext : IExecutionContext
    {
        /// <summary>
        /// Gets the schema being used to process the records.
        /// </summary>
        new SeparatedValueSchema Schema { get; }

        /// <summary>
        /// Gets the global options being used to process the records.
        /// </summary>
        new SeparatedValueOptions Options { get; }
    }

    internal class FixedLengthExecutionContext : IFixedLengthExecutionContext
    {
        public FixedLengthSchema Schema { get; set; }

        public FixedLengthOptions Options { get; set; }

        ISchema IExecutionContext.Schema => Schema;

        IOptions IExecutionContext.Options => Options;
    }

    internal class SeparatedValueExecutionContext : ISeparatedValueExecutionContext
    {
        public SeparatedValueSchema Schema { get; set; }

        public SeparatedValueOptions Options { get; set; }

        ISchema IExecutionContext.Schema => Schema;

        IOptions IExecutionContext.Options => Options;
    }
}
