namespace FlatFiles
{
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
}
