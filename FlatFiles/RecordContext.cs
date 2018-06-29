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
    }

    /// <summary>
    /// Holds information about the record currently being processed.
    /// </summary>
    public interface IFixedLengthRecordContext : IRecordContext
    {
        /// <summary>
        /// Gets information about the currently running process.
        /// </summary>
        new IFixedLengthExecutionContext ExecutionContext { get; }
    }

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

    internal class FixedLengthRecordContext : IFixedLengthRecordContext
    {
        public FixedLengthExecutionContext ExecutionContext { get; set; }

        public int PhysicalRecordNumber { get; set; }

        public int LogicalRecordNumber { get; set; }

        IFixedLengthExecutionContext IFixedLengthRecordContext.ExecutionContext => ExecutionContext;

        IExecutionContext IRecordContext.ExecutionContext => ExecutionContext;
    }

    internal class SeparatedValueRecordContext : ISeparatedValueRecordContext
    {
        public SeparatedValueExecutionContext ExecutionContext { get; set; }

        public int PhysicalRecordNumber { get; set; }

        public int LogicalRecordNumber { get; set; }

        ISeparatedValueExecutionContext ISeparatedValueRecordContext.ExecutionContext => ExecutionContext;

        IExecutionContext IRecordContext.ExecutionContext => ExecutionContext;
    }

    internal class RecordContext : IRecordContext
    {
        public IExecutionContext ExecutionContext { get; set; }

        public int PhysicalRecordNumber { get; set; }

        public int LogicalRecordNumber { get; set; }
    }
}
