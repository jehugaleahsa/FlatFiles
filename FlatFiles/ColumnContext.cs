namespace FlatFiles
{
    internal class ColumnContext : IColumnContext
    {
        public ColumnContext(IRecordContext recordContext)
        {
            RecordContext = recordContext;
        }

        public IRecordContext RecordContext { get; set; }

        public IColumnDefinition ColumnDefinition
        {
            get
            {
                var schema = RecordContext.ExecutionContext.Schema;
                var definition = schema.ColumnDefinitions[PhysicalIndex];
                return definition;
            }
        }

        public int PhysicalIndex { get; set; }

        public int LogicalIndex { get; set; }
    }
}
