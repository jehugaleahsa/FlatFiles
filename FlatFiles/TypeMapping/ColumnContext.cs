using System;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Holds information about the column currently being processed.
    /// </summary>
    public interface IColumnContext
    {
        /// <summary>
        /// Gets information about the record currently being processed.
        /// </summary>
        IRecordContext RecordContext { get; }

        /// <summary>
        /// Gets the physical index into the underlying file.
        /// </summary>
        int PhysicalIndex { get; }

        /// <summary>
        /// Gets the logical index into the underlying file, excluding ignored columns.
        /// </summary>
        int LogicalIndex { get; }
    }

    internal class ColumnContext : IColumnContext
    {
        public IRecordContext RecordContext { get; set; }

        public int PhysicalIndex { get; set; }

        public int LogicalIndex { get; set; }
    }
}
