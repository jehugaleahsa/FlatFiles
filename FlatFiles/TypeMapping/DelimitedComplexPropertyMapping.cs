using System;
using FlatFiles.Properties;

namespace FlatFiles.TypeMapping
{
    internal sealed class DelimitedComplexPropertyMapping<TEntity> : IDelimitedComplexPropertyMapping, IMemberMapping
    {
        private readonly IDelimitedTypeMapper<TEntity> mapper;
        private string columnName;
        private DelimitedOptions? options;
        private INullFormatter nullFormatter = FlatFiles.NullFormatter.Default;
        private IDefaultValue defaultValue = FlatFiles.DefaultValue.Disabled();
        private bool isNullable = true;
        private Func<string, string?>? preprocessor;
        private Func<IColumnContext?, string, string?>? onParsing;
        private Func<IColumnContext?, object?, object?>? onParsed;
        private Func<IColumnContext?, object?, object?>? onFormatting;
        private Func<IColumnContext?, string, string?>? onFormatted;

        public DelimitedComplexPropertyMapping(
            IDelimitedTypeMapper<TEntity> mapper, 
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
                DelimitedSchema schema = mapper.GetSchema();
                var column = new DelimitedComplexColumn(columnName, schema)
                {
                    Options = options,
                    NullFormatter = nullFormatter,
                    DefaultValue = defaultValue,
                    IsNullable = isNullable,
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
                return new ComplexMapperColumn<TEntity>(schema, options ?? new DelimitedOptions(), column, recordMapper);
            }
        }

        public IMemberAccessor Member { get; }

        public Action<IColumnContext?, object?, object?>? Reader => null;

        public Action<IColumnContext?, object?, object?[]>? Writer => null;

        public int PhysicalIndex { get; }

        public int LogicalIndex { get; }

        public IDelimitedComplexPropertyMapping ColumnName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(Resources.BlankColumnName);
            }
            columnName = name;
            return this;
        }

        public IDelimitedComplexPropertyMapping WithOptions(DelimitedOptions? options)
        {
            this.options = options;
            return this;
        }

        public IDelimitedComplexPropertyMapping NullFormatter(INullFormatter formatter)
        {
            nullFormatter = formatter ?? FlatFiles.NullFormatter.Default;
            return this;
        }

        public IDelimitedComplexPropertyMapping DefaultValue(IDefaultValue defaultValue)
        {
            this.defaultValue = defaultValue ?? FlatFiles.DefaultValue.Disabled();
            return this;
        }

        public IDelimitedComplexPropertyMapping Nullable(bool isNullable)
        {
            this.isNullable = isNullable;
            return this;
        }

        public IDelimitedComplexPropertyMapping Preprocessor(Func<string, string?>? preprocessor)
        {
            this.preprocessor = preprocessor;
            return this;
        }

        public IDelimitedComplexPropertyMapping OnParsing(Func<IColumnContext?, string, string?>? handler)
        {
            this.onParsing = handler;
            return this;
        }

        public IDelimitedComplexPropertyMapping OnParsed(Func<IColumnContext?, object?, object?>? handler)
        {
            this.onParsed = handler;
            return this;
        }

        public IDelimitedComplexPropertyMapping OnFormatting(Func<IColumnContext?, object?, object?>? handler)
        {
            this.onFormatting = handler;
            return this;
        }

        public IDelimitedComplexPropertyMapping OnFormatted(Func<IColumnContext?, string, string?>? handler)
        {
            this.onFormatted = handler;
            return this;
        }
    }
}
