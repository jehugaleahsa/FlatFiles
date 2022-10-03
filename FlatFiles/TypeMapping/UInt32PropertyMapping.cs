using System;
using System.Globalization;

namespace FlatFiles.TypeMapping
{
    internal sealed class UInt32PropertyMapping : IUInt32PropertyMapping, IMemberMapping
    {
        private readonly UInt32Column column;

        public UInt32PropertyMapping(UInt32Column column, IMemberAccessor member, int physicalIndex, int logicalIndex)
        {
            this.column = column;
            Member = member;
            PhysicalIndex = physicalIndex;
            LogicalIndex = logicalIndex;
        }

        public IUInt32PropertyMapping ColumnName(string name)
        {
            column.ColumnName = name;
            return this;
        }

        public IUInt32PropertyMapping FormatProvider(IFormatProvider? provider)
        {
            column.FormatProvider = provider;
            return this;
        }

        public IUInt32PropertyMapping NumberStyles(NumberStyles styles)
        {
            column.NumberStyles = styles;
            return this;
        }

        public IUInt32PropertyMapping OutputFormat(string? format)
        {
            column.OutputFormat = format;
            return this;
        }

        public IUInt32PropertyMapping NullFormatter(INullFormatter formatter)
        {
            column.NullFormatter = formatter;
            return this;
        }

        public IUInt32PropertyMapping DefaultValue(IDefaultValue defaultValue)
        {
            column.DefaultValue = defaultValue;
            return this;
        }

        public IUInt32PropertyMapping Nullable(bool isNullable)
        {
            column.IsNullable = isNullable;
            return this;
        }

        public IUInt32PropertyMapping Preprocessor(Func<string, string?>? preprocessor)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            column.Preprocessor = preprocessor;
#pragma warning restore CS0618 // Type or member is obsolete
            return this;
        }

        public IUInt32PropertyMapping OnParsing(Func<IColumnContext?, string, string?>? handler)
        {
            column.OnParsing = handler;
            return this;
        }

        public IUInt32PropertyMapping OnParsed(Func<IColumnContext?, object?, object?>? handler)
        {
            column.OnParsed = handler;
            return this;
        }

        public IUInt32PropertyMapping OnFormatting(Func<IColumnContext?, object?, object?>? handler)
        {
            column.OnFormatting = handler;
            return this;
        }

        public IUInt32PropertyMapping OnFormatted(Func<IColumnContext?, string, string?>? handler)
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
