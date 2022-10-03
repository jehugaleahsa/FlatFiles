using System;

namespace FlatFiles.TypeMapping
{
    internal sealed class StringPropertyMapping : IStringPropertyMapping, IMemberMapping
    {
        private readonly StringColumn column;

        public StringPropertyMapping(StringColumn column, IMemberAccessor member, int physicalIndex, int logicalIndex)
        {
            this.column = column;
            Member = member;
            PhysicalIndex = physicalIndex;
            LogicalIndex = logicalIndex;
        }

        public IStringPropertyMapping ColumnName(string name)
        {
            column.ColumnName = name;
            return this;
        }

        public IStringPropertyMapping Trim(bool trim)
        {
            column.Trim = trim;
            return this;
        }

        public IStringPropertyMapping NullFormatter(INullFormatter formatter)
        {
            column.NullFormatter = formatter;
            return this;
        }

        public IStringPropertyMapping DefaultValue(IDefaultValue defaultValue)
        {
            column.DefaultValue = defaultValue;
            return this;
        }

        public IStringPropertyMapping Nullable(bool isNullable)
        {
            column.IsNullable = isNullable;
            return this;
        }

        public IStringPropertyMapping Preprocessor(Func<string, string?>? preprocessor)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            column.Preprocessor = preprocessor;
#pragma warning restore CS0618 // Type or member is obsolete
            return this;
        }

        public IStringPropertyMapping OnParsing(Func<IColumnContext?, string, string?>? handler)
        {
            column.OnParsing = handler;
            return this;
        }

        public IStringPropertyMapping OnParsed(Func<IColumnContext?, object?, object?>? handler)
        {
            column.OnParsed = handler;
            return this;
        }

        public IStringPropertyMapping OnFormatting(Func<IColumnContext?, object?, object?>? handler)
        {
            column.OnFormatting = handler;
            return this;
        }

        public IStringPropertyMapping OnFormatted(Func<IColumnContext?, string, string?>? handler)
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
