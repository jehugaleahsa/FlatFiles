namespace FlatFiles
{
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
}
