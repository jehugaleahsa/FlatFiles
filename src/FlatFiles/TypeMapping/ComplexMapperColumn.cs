using System;

namespace FlatFiles.TypeMapping
{
    internal class ComplexMapperColumn<TEntity> : IColumnDefinition
    {
        private readonly IColumnDefinition column;
        private readonly TypedRecordReader<TEntity> reader;
        private readonly TypedRecordWriter<TEntity> writer;

        public ComplexMapperColumn(IColumnDefinition column, IRecordMapper<TEntity> mapper)
        {
            this.column = column;
            this.reader = mapper.GetReader();
            this.writer = mapper.GetWriter();
        }

        public string ColumnName
        {
            get { return column.ColumnName; }
        }

        public Type ColumnType
        {
            get { return typeof(TEntity); }
        }

        public bool IsIgnored
        {
            get { return column.IsIgnored; }
        }

        public INullHandler NullHandler
        {
            get { return column.NullHandler; }
            set { column.NullHandler = value; }
        }

        public Func<string, string> Preprocessor
        {
            get { return column.Preprocessor; }
            set { column.Preprocessor = value; }
        }

        public object Parse(string value)
        {
            object parsed = column.Parse(value);
            object[] values = parsed as object[];
            TEntity result = reader.Read(values);
            return result;
        }

        public string Format(object value)
        {
            object[] values = new object[writer.MemberCount];
            writer.Write((TEntity)value, values);
            string formatted = column.Format(values);
            return formatted;
        }
    }
}
