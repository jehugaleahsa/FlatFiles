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
}
