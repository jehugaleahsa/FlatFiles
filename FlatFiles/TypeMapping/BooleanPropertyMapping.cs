using System;

namespace FlatFiles.TypeMapping
{
    internal sealed class BooleanPropertyMapping : IBooleanPropertyMapping, IMemberMapping
    {
        private readonly BooleanColumn column;

        public BooleanPropertyMapping(BooleanColumn column, IMemberAccessor member, int physicalIndex, int logicalIndex)
        {
            this.column = column;
            Member = member;
            PhysicalIndex = physicalIndex;
            LogicalIndex = logicalIndex;
        }

        public IBooleanPropertyMapping ColumnName(string name)
        {
            column.ColumnName = name;
            return this;
        }

        public IBooleanPropertyMapping TrueString(string? value)
        {
            column.TrueString = value;
            return this;
        }

        public IBooleanPropertyMapping FalseString(string? value)
        {
            column.FalseString = value;
            return this;
        }

        public IBooleanPropertyMapping NullFormatter(INullFormatter formatter)
        {
            column.NullFormatter = formatter;
            return this;
        }

        public IBooleanPropertyMapping DefaultValue(IDefaultValue defaultValue)
        {
            column.DefaultValue = defaultValue;
            return this;
        }

        public IBooleanPropertyMapping Nullable(bool isNullable)
        {
            column.IsNullable = isNullable;
            return this;
        }

        public IBooleanPropertyMapping Preprocessor(Func<string, string?>? preprocessor)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            column.Preprocessor = preprocessor;
#pragma warning restore CS0618 // Type or member is obsolete
            return this;
        }

        public IBooleanPropertyMapping OnParsing(Func<IColumnContext?, string, string?>? handler)
        {
            column.OnParsing = handler;
            return this;
        }

        public IBooleanPropertyMapping OnParsed(Func<IColumnContext?, object?, object?>? handler)
        {
            column.OnParsed = handler;
            return this;
        }

        public IBooleanPropertyMapping OnFormatting(Func<IColumnContext?, object?, object?>? handler)
        {
            column.OnFormatting = handler;
            return this;
        }

        public IBooleanPropertyMapping OnFormatted(Func<IColumnContext?, string, string?>? handler)
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
