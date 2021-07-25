using System;

namespace FlatFiles.TypeMapping
{
    internal sealed class EnumPropertyMapping<TEnum> : IEnumPropertyMapping<TEnum>, IMemberMapping
        where TEnum : Enum
    {
        private readonly EnumColumn<TEnum> column;

        public EnumPropertyMapping(EnumColumn<TEnum> column, IMemberAccessor member, int physicalIndex, int logicalIndex)
        {
            this.column = column;
            Member = member;
            PhysicalIndex = physicalIndex;
            LogicalIndex = logicalIndex;
        }

        public IEnumPropertyMapping<TEnum> ColumnName(string name)
        {
            column.ColumnName = name;
            return this;
        }

        public IEnumPropertyMapping<TEnum> Parser(Func<string, TEnum>? parser)
        {
            column.Parser = parser;
            return this;
        }

        public IEnumPropertyMapping<TEnum> Formatter(Func<TEnum, string?>? formatter)
        {
            column.Formatter = formatter;
            return this;
        }

        public IEnumPropertyMapping<TEnum> NullFormatter(INullFormatter formatter)
        {
            column.NullFormatter = formatter;
            return this;
        }

        public IEnumPropertyMapping<TEnum> DefaultValue(IDefaultValue defaultValue)
        {
            column.DefaultValue = defaultValue;
            return this;
        }

        public IEnumPropertyMapping<TEnum> Preprocessor(Func<string, string?>? preprocessor)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            column.Preprocessor = preprocessor;
#pragma warning restore CS0618 // Type or member is obsolete
            return this;
        }

        public IEnumPropertyMapping<TEnum> OnParsing(Func<IColumnContext?, string, string?>? handler)
        {
            column.OnParsing = handler;
            return this;
        }

        public IEnumPropertyMapping<TEnum> OnParsed(Func<IColumnContext?, object?, object?>? handler)
        {
            column.OnParsed = handler;
            return this;
        }

        public IEnumPropertyMapping<TEnum> OnFormatting(Func<IColumnContext?, object?, object?>? handler)
        {
            column.OnFormatting = handler;
            return this;
        }

        public IEnumPropertyMapping<TEnum> OnFormatted(Func<IColumnContext?, string, string?>? handler)
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
