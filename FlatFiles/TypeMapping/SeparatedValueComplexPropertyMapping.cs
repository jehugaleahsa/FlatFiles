using System;
using FlatFiles.Properties;

namespace FlatFiles.TypeMapping
{
    internal sealed class SeparatedValueComplexPropertyMapping<TEntity> : ISeparatedValueComplexPropertyMapping, IMemberMapping
    {
        private readonly ISeparatedValueTypeMapper<TEntity> mapper;
        private string columnName;
        private SeparatedValueOptions? options;
        private INullFormatter nullFormatter = FlatFiles.NullFormatter.Default;
        private IDefaultValue defaultValue = FlatFiles.DefaultValue.Disabled();
        private Func<string, string?>? preprocessor;
        private Func<IColumnContext?, string, string?>? onParsing;
        private Func<IColumnContext?, object?, object?>? onParsed;
        private Func<IColumnContext?, object?, object?>? onFormatting;
        private Func<IColumnContext?, string, string?>? onFormatted;

        public SeparatedValueComplexPropertyMapping(
            ISeparatedValueTypeMapper<TEntity> mapper, 
            IMemberAccessor member, 
            int physicalIndex, 
            int logicalIndex)
        {
            this.mapper = mapper;
            Member = member;
            columnName = member.Name;
            PhysicalIndex = physicalIndex;
            LogicalIndex = logicalIndex;
        }

        public IColumnDefinition ColumnDefinition
        {
            get
            {
                SeparatedValueSchema schema = mapper.GetSchema();
                var column = new SeparatedValueComplexColumn(columnName, schema)
                {
                    Options = options,
                    NullFormatter = nullFormatter,
                    DefaultValue = defaultValue,
#pragma warning disable CS0618 // Type or member is obsolete
                    Preprocessor = preprocessor,
#pragma warning restore CS0618 // Type or member is obsolete
                    OnParsing = onParsing,
                    OnParsed = onParsed,
                    OnFormatting = onFormatting,
                    OnFormatted = onFormatted
                };

                var mapperSource = (IMapperSource<TEntity>)mapper;
                var recordMapper = mapperSource.GetMapper();
                return new ComplexMapperColumn<TEntity>(schema, options ?? new SeparatedValueOptions(), column, recordMapper);
            }
        }

        public IMemberAccessor Member { get; }

        public Action<IColumnContext?, object?, object?>? Reader => null;

        public Action<IColumnContext?, object?, object?[]>? Writer => null;

        public int PhysicalIndex { get; }

        public int LogicalIndex { get; }

        public ISeparatedValueComplexPropertyMapping ColumnName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(Resources.BlankColumnName);
            }
            columnName = name;
            return this;
        }

        public ISeparatedValueComplexPropertyMapping WithOptions(SeparatedValueOptions? options)
        {
            this.options = options;
            return this;
        }

        public ISeparatedValueComplexPropertyMapping NullFormatter(INullFormatter formatter)
        {
            nullFormatter = formatter ?? FlatFiles.NullFormatter.Default;
            return this;
        }

        public ISeparatedValueComplexPropertyMapping DefaultValue(IDefaultValue defaultValue)
        {
            this.defaultValue = defaultValue ?? FlatFiles.DefaultValue.Disabled();
            return this;
        }

        public ISeparatedValueComplexPropertyMapping Preprocessor(Func<string, string?>? preprocessor)
        {
            this.preprocessor = preprocessor;
            return this;
        }

        public ISeparatedValueComplexPropertyMapping OnParsing(Func<IColumnContext?, string, string?>? handler)
        {
            this.onParsing = handler;
            return this;
        }

        public ISeparatedValueComplexPropertyMapping OnParsed(Func<IColumnContext?, object?, object?>? handler)
        {
            this.onParsed = handler;
            return this;
        }

        public ISeparatedValueComplexPropertyMapping OnFormatting(Func<IColumnContext?, object?, object?>? handler)
        {
            this.onFormatting = handler;
            return this;
        }

        public ISeparatedValueComplexPropertyMapping OnFormatted(Func<IColumnContext?, string, string?>? handler)
        {
            this.onFormatted = handler;
            return this;
        }
    }
}
