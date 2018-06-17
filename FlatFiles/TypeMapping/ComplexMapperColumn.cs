using System;

namespace FlatFiles.TypeMapping
{
    internal class ComplexMapperColumn<TEntity> : IColumnDefinition
    {
        private readonly IColumnDefinition _column;
        private readonly Func<object[], TEntity> _reader;
        private readonly Action<TEntity, object[]> _writer;
        private readonly int _workCount;

        public ComplexMapperColumn(IColumnDefinition column, IMapper<TEntity> mapper)
        {
            _column = column;
            _reader = mapper.GetReader();
            _writer = mapper.GetWriter();
            _workCount = mapper.WorkCount;
        }

        public string ColumnName => _column.ColumnName;

        public Type ColumnType => typeof(TEntity);

        public bool IsIgnored => _column.IsIgnored;

        public INullHandler NullHandler
        {
            get => _column.NullHandler;
            set => _column.NullHandler = value;
        }

        public Func<string, string> Preprocessor
        {
            get => _column.Preprocessor;
            set => _column.Preprocessor = value;
        }

        public object Parse(string value)
        {
            object parsed = _column.Parse(value);
            object[] values = parsed as object[];
            TEntity result = _reader(values);
            return result;
        }

        public string Format(object value)
        {
            object[] values = new object[_workCount];
            _writer((TEntity)value, values);
            string formatted = _column.Format(values);
            return formatted;
        }
    }
}
