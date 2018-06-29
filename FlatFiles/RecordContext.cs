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
        IProcessContext ProcessContext { get; }

        /// <summary>
        /// Gets the index of the record being processed.
        /// </summary>
        int PhysicalRecordNumber { get; }

        /// <summary>
        /// Gets the index of the record being processed, ignoring skipped records.
        /// </summary>
        int LogicalRecordNumber { get; }
    }

    /// <summary>
    /// Holds information about the record currently being processed.
    /// </summary>
    public interface IFixedLengthRecordContext : IRecordContext
    {
        /// <summary>
        /// Gets information about the currently running process.
        /// </summary>
        new IFixedLengthProcessContext ProcessContext { get; }
    }

    /// <summary>
    /// Holds information about the record currently being processed.
    /// </summary>
    public interface ISeparatedValueRecordContext : IRecordContext
    {
        /// <summary>
        /// Gets information about the currently running process.
        /// </summary>
        new ISeparatedValueProcessContext ProcessContext { get; }
    }

    internal class FixedLengthRecordContext : IFixedLengthRecordContext
    {
        public FixedLengthProcessContext ProcessContext { get; set; }

        public int PhysicalRecordNumber { get; set; }

        public int LogicalRecordNumber { get; set; }

        IFixedLengthProcessContext IFixedLengthRecordContext.ProcessContext => ProcessContext;

        IProcessContext IRecordContext.ProcessContext => ProcessContext;
    }

    internal class SeparatedValueRecordContext : ISeparatedValueRecordContext
    {
        public SeparatedValueProcessContext ProcessContext { get; set; }

        public int PhysicalRecordNumber { get; set; }

        public int LogicalRecordNumber { get; set; }

        ISeparatedValueProcessContext ISeparatedValueRecordContext.ProcessContext => ProcessContext;

        IProcessContext IRecordContext.ProcessContext => ProcessContext;
    }

    internal class RecordContext : IRecordContext
    {
        public IProcessContext ProcessContext { get; set; }

        public int PhysicalRecordNumber { get; set; }

        public int LogicalRecordNumber { get; set; }
    }
}
