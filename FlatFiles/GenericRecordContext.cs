namespace FlatFiles
{
    internal class GenericRecordContext : IRecordContext
    {
        public GenericRecordContext(IExecutionContext context)
        {
            ExecutionContext = context;
        }

        public IExecutionContext ExecutionContext { get; }

        public int PhysicalRecordNumber { get; set; }

        public int LogicalRecordNumber { get; set; }

        public string? Record { get; set; }

        public string[]? Values { get; set; }
    }
}
