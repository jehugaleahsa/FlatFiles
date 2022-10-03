using System;
using System.Text;

namespace FlatFiles.TypeMapping
{
    internal sealed class ByteArrayPropertyMapping : IByteArrayPropertyMapping, IMemberMapping
    {
        private readonly ByteArrayColumn column;

        public ByteArrayPropertyMapping(ByteArrayColumn column, IMemberAccessor member, int physicalIndex, int logicalIndex)
        {
            this.column = column;
            Member = member;
            PhysicalIndex = physicalIndex;
            LogicalIndex = logicalIndex;
        }

        public IByteArrayPropertyMapping ColumnName(string name)
        {
            column.ColumnName = name;
            return this;
        }

        public IByteArrayPropertyMapping Encoding(Encoding? encoding)
        {
            column.Encoding = encoding;
            return this;
        }

        public IByteArrayPropertyMapping NullFormatter(INullFormatter formatter)
        {
            column.NullFormatter = formatter;
            return this;
        }

        public IByteArrayPropertyMapping DefaultValue(IDefaultValue defaultValue)
        {
            column.DefaultValue = defaultValue;
            return this;
        }

        public IByteArrayPropertyMapping Nullable(bool isNullable)
        {
            column.IsNullable = isNullable;
            return this;
        }

        public IByteArrayPropertyMapping Preprocessor(Func<string, string?>? preprocessor)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            column.Preprocessor = preprocessor;
#pragma warning restore CS0618 // Type or member is obsolete
            return this;
        }

        public IByteArrayPropertyMapping OnParsing(Func<IColumnContext?, string, string?>? handler)
        {
            column.OnParsing = handler;
            return this;
        }

        public IByteArrayPropertyMapping OnParsed(Func<IColumnContext?, object?, object?>? handler)
        {
            column.OnParsed = handler;
            return this;
        }

        public IByteArrayPropertyMapping OnFormatting(Func<IColumnContext?, object?, object?>? handler)
        {
            column.OnFormatting = handler;
            return this;
        }

        public IByteArrayPropertyMapping OnFormatted(Func<IColumnContext?, string, string?>? handler)
        {
            column.OnFormatted = handler;
            return this;
        }

        public IMemberAccessor Member { get; }

        public Action<IColumnContext?, object?, object?>? Reader => null;

        public Action<IColumnContext?, object?, object?[]>? Writer => null;

        public IColumnDefinition ColumnDefinition => column;

        public int PhysicalIndex { get; }

        public int LogicalIndex { get; }
    }
}
