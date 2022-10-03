using System;
using System.Globalization;

namespace FlatFiles.TypeMapping
{
    internal sealed class BytePropertyMapping : IBytePropertyMapping, IMemberMapping
    {
        private readonly ByteColumn column;

        public BytePropertyMapping(ByteColumn column, IMemberAccessor member, int physicalIndex, int logicalIndex)
        {
            this.column = column;
            Member = member;
            PhysicalIndex = physicalIndex;
            LogicalIndex = logicalIndex;
        }

        public IBytePropertyMapping ColumnName(string name)
        {
            column.ColumnName = name;
            return this;
        }

        public IBytePropertyMapping FormatProvider(IFormatProvider? provider)
        {
            column.FormatProvider = provider;
            return this;
        }

        public IBytePropertyMapping NumberStyles(NumberStyles styles)
        {
            column.NumberStyles = styles;
            return this;
        }

        public IBytePropertyMapping OutputFormat(string? format)
        {
            column.OutputFormat = format;
            return this;
        }

        public IBytePropertyMapping NullFormatter(INullFormatter formatter)
        {
            column.NullFormatter = formatter;
            return this;
        }

        public IBytePropertyMapping DefaultValue(IDefaultValue defaultValue)
        {
            column.DefaultValue = defaultValue;
            return this;
        }

        public IBytePropertyMapping Nullable(bool isNullable)
        {
            column.IsNullable = isNullable;
            return this;
        }

        public IBytePropertyMapping Preprocessor(Func<string, string?>? preprocessor)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            column.Preprocessor = preprocessor;
#pragma warning restore CS0618 // Type or member is obsolete
            return this;
        }

        public IBytePropertyMapping OnParsing(Func<IColumnContext?, string, string?>? handler)
        {
            column.OnParsing = handler;
            return this;
        }

        public IBytePropertyMapping OnParsed(Func<IColumnContext?, object?, object?>? handler)
        {
            column.OnParsed = handler;
            return this;
        }

        public IBytePropertyMapping OnFormatting(Func<IColumnContext?, object?, object?>? handler)
        {
            column.OnFormatting = handler;
            return this;
        }

        public IBytePropertyMapping OnFormatted(Func<IColumnContext?, string, string?>? handler)
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
