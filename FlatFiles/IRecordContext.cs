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

    internal interface IRecoverableRecordContext : IRecordContext
    {
        event EventHandler<ColumnErrorEventArgs> ColumnError;

        bool HasHandler { get; }

        void ProcessError(object sender, ColumnErrorEventArgs e);
    }

    internal class FixedLengthRecordContext : IFixedLengthRecordContext, IRecoverableRecordContext
    {
        public event EventHandler<ColumnErrorEventArgs> ColumnError;

        public FixedLengthExecutionContext ExecutionContext { get; set; }

        public int PhysicalRecordNumber { get; set; }

        public int LogicalRecordNumber { get; set; }

        public string Record { get; set; }

        public string[] Values { get; set; }

        IFixedLengthExecutionContext IFixedLengthRecordContext.ExecutionContext => ExecutionContext;

        IExecutionContext IRecordContext.ExecutionContext => ExecutionContext;

        public bool HasHandler => ColumnError != null;

        public void ProcessError(object sender, ColumnErrorEventArgs e)
        {
            ColumnError?.Invoke(sender, e);
        }
    }

    internal class SeparatedValueRecordContext : ISeparatedValueRecordContext, IRecoverableRecordContext
    {
        public event EventHandler<ColumnErrorEventArgs> ColumnError;

        public SeparatedValueExecutionContext ExecutionContext { get; set; }

        public int PhysicalRecordNumber { get; set; }

        public int LogicalRecordNumber { get; set; }

        public string Record { get; set; }

        public string[] Values { get; set; }

        ISeparatedValueExecutionContext ISeparatedValueRecordContext.ExecutionContext => ExecutionContext;

        IExecutionContext IRecordContext.ExecutionContext => ExecutionContext;

        public bool HasHandler => ColumnError != null;

        public void ProcessError(object sender, ColumnErrorEventArgs e)
        {
            ColumnError?.Invoke(sender, e);
        }
    }
}
