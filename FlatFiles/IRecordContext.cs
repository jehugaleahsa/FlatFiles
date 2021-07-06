using System;

namespace FlatFiles
{
    /// <summary>
    /// Holds information about the record currently being processed.
    /// </summary>
    public interface IRecordContext
    {
        /// <summary>
        /// Gets information about the currently running process.
        /// </summary>
        IExecutionContext ExecutionContext { get; }

        /// <summary>
        /// Gets the index of the record being processed.
        /// </summary>
        int PhysicalRecordNumber { get; }

        /// <summary>
        /// Gets the index of the record being processed, ignoring skipped records.
        /// </summary>
        int LogicalRecordNumber { get; }

        /// <summary>
        /// Gets the record being processed when the error occurred.
        /// </summary>
        string Record { get; }

        /// <summary>
        /// Gets the partitioned values being processed when the error occurred.
        /// </summary>
        string[] Values { get; }
    }
}
