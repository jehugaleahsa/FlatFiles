using System;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Holds information about the record currently being processed.
    /// </summary>
    public interface IRecordContext
    {
        /// <summary>
        /// Gets information about the currently running process.
        /// </summary>
        IProcessContext ProcessContext { get; }

        /// <summary>
        /// Gets the index of the record being processed.
        /// </summary>
        int PhysicalCount { get; }

        /// <summary>
        /// Gets the index of the record being processed, ignoring skipped records.
        /// </summary>
        int LogicalCount { get; }
    }

    internal class RecordContext : IRecordContext
    {
        public IProcessContext ProcessContext { get; set; }

        public int PhysicalCount { get; set; }

        public int LogicalCount { get; set; }
    }
}
