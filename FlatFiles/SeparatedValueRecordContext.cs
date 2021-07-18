using System;

namespace FlatFiles
{
    internal sealed class SeparatedValueRecordContext : ISeparatedValueRecordContext, IRecoverableRecordContext
    {
        public SeparatedValueRecordContext(SeparatedValueExecutionContext executionContext)
        {
            ExecutionContext = executionContext;
        }

        public event EventHandler<ColumnErrorEventArgs>? ColumnError;

        public SeparatedValueExecutionContext ExecutionContext { get; set; }

        public int PhysicalRecordNumber { get; set; }

        public int LogicalRecordNumber { get; set; }

        public string? Record { get; set; }

        public string[]? Values { get; set; }

        ISeparatedValueExecutionContext ISeparatedValueRecordContext.ExecutionContext => ExecutionContext;

        IExecutionContext IRecordContext.ExecutionContext => ExecutionContext;

        public bool HasHandler => ColumnError != null;

        public void ProcessError(object sender, ColumnErrorEventArgs e)
        {
            ColumnError?.Invoke(sender, e);
        }
    }
}
