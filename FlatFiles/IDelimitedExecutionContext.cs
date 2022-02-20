namespace FlatFiles
{
    /// <summary>
    /// Holds information about the currently running process.
    /// </summary>
    public interface IDelimitedExecutionContext : IExecutionContext
    {
        /// <summary>
        /// Gets the schema being used to process the records.
        /// </summary>
        new DelimitedSchema? Schema { get; }

        /// <summary>
        /// Gets the global options being used to process the records.
        /// </summary>
        new DelimitedOptions Options { get; }
    }
}
