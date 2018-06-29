using System;

namespace FlatFiles.TypeMapping
{
    internal class ComplexMapperColumn<TEntity> : IColumnDefinition
    {
        private readonly IColumnDefinition column;
        private readonly Func<IRecordContext, object[], TEntity> reader;
        private readonly Action<IRecordContext, TEntity, object[]> writer;
        private readonly int logicalCount;

        public ComplexMapperColumn(IColumnDefinition column, IMapper<TEntity> mapper)
        {
            this.column = column;
            reader = mapper.GetReader();
            writer = mapper.GetWriter();
            logicalCount = mapper.LogicalCount;
        }

        public string ColumnName => column.ColumnName;

        public Type ColumnType => typeof(TEntity);

        public bool IsIgnored => column.IsIgnored;

        public INullHandler NullHandler
        {
            get => column.NullHandler;
            set => column.NullHandler = value;
        }

        public Func<string, string> Preprocessor
        {
            get => column.Preprocessor;
            set => column.Preprocessor = value;
        }

        public object Parse(string value)
        {
            object parsed = column.Parse(value);
            object[] values = parsed as object[];
            TEntity result = reader(null, values);
            return result;
        }

        public string Format(object value)
        {
            object[] values = new object[logicalCount];
            writer(null, (TEntity)value, values);
            string formatted = column.Format(values);
            return formatted;
        }
    }
}
