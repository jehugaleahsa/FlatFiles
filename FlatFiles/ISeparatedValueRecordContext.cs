namespace FlatFiles
{
    /// <summary>
    /// Holds information about the record currently being processed.
    /// </summary>
    public interface ISeparatedValueRecordContext : IRecordContext
    {
        /// <summary>
        /// Gets information about the currently running process.
        /// </summary>
        new ISeparatedValueExecutionContext ExecutionContext { get; }
    }
}
