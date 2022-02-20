namespace FlatFiles
{
    /// <summary>
    /// Holds information about the record currently being processed.
    /// </summary>
    public interface IDelimitedRecordContext : IRecordContext
    {
        /// <summary>
        /// Gets information about the currently running process.
        /// </summary>
        new IDelimitedExecutionContext ExecutionContext { get; }
    }
}
