using FlatFiles.Properties;

namespace FlatFiles
{
    internal sealed class ColumnContext : IColumnContext
    {
        public ColumnContext(IRecordContext recordContext, int physicalIndex, int logicalIndex)
        {
            var schema = recordContext.ExecutionContext.Schema;
            if (schema == null)
            {
                throw new FlatFileException(Resources.SchemaNotDefined);
            }
            RecordContext = recordContext;
            ColumnDefinition = schema.ColumnDefinitions[physicalIndex];
            PhysicalIndex = physicalIndex;
            LogicalIndex = logicalIndex;
        }

        public IRecordContext RecordContext { get; }

        public IColumnDefinition ColumnDefinition { get; }

        public int PhysicalIndex { get; }

        public int LogicalIndex { get; }
    }
}
