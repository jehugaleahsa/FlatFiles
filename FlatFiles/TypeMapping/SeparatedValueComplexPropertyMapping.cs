using System;
using FlatFiles.Properties;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents the mapping from a type property to an object.
    /// </summary>
    public interface ISeparatedValueComplexPropertyMapping
    {
        /// <summary>
        /// Sets the name of the column in the input or output file.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The property mapping for further configuration.</returns>
        ISeparatedValueComplexPropertyMapping ColumnName(string name);

        /// <summary>
        /// Sets the options to use when reading/writing the complex type.
        /// </summary>
        /// <param name="options">The options to use.</param>
        /// <returns>The property mapping for further configuration.</returns>
        ISeparatedValueComplexPropertyMapping WithOptions(SeparatedValueOptions options);

        /// <summary>
        /// Sets what value(s) are treated as null.
        /// </summary>
        /// <param name="formatter">The formatter to use.</param>
        /// <returns>The property mapping for further configuration.</returns>
        /// <remarks>Passing null will cause the default formatter to be used.</remarks>
        ISeparatedValueComplexPropertyMapping NullFormatter(INullFormatter formatter);

        /// <summary>
        /// Sets the default value to use when a null is encountered on a non-null property.
        /// </summary>
        /// <param name="defaultValue">The default value to use.</param>
        /// <returns>The property mapping for further configuration.</returns>
        /// <remarks>Passing null will cause an exception to be thrown for unexpected nulls.</remarks>
        ISeparatedValueComplexPropertyMapping DefaultValue(IDefaultValue defaultValue);

        /// <summary>
        /// Sets a function to preprocess in the input before parsing it.
        /// </summary>
        /// <param name="preprocessor">A preprocessor function.</param>
        /// <returns>The property mapping for further configuration.</returns>
        [Obsolete("This function has been superseded by the OnParsing function.")]
        ISeparatedValueComplexPropertyMapping Preprocessor(Func<string, string> preprocessor);

        /// <summary>
        /// Sets the function to run before the input is parsed.
        /// </summary>
        /// <param name="handler">A function to call before the textual value is parsed.</param>
        /// <returns>The property mapping for further configuration.</returns>
        ISeparatedValueComplexPropertyMapping OnParsing(Func<IColumnContext, String, String> handler);

        /// <summary>
        /// Sets the function to run after the input is parsed.
        /// </summary>
        /// <param name="handler">A function to call after the value is parsed.</param>
        /// <returns>The property mapping for further configuration.</returns>
        ISeparatedValueComplexPropertyMapping OnParsed(Func<IColumnContext, object, object> handler);

        /// <summary>
        /// Sets the function to run before the output is formatted as a string.
        /// </summary>1
        /// <param name="handler">A function to call before the value is formatted as a string.</param>
        /// <returns>The property mapping for further configuration.</returns>
        ISeparatedValueComplexPropertyMapping OnFormatting(Func<IColumnContext, object, object> handler);

        /// <summary>
        /// Sets the function to run after the output is formatted as a string.
        /// </summary>
        /// <param name="handler">A function to call after the value is formatted as a string.</param>
        /// <returns>The property mapping for further configuration.</returns>
        ISeparatedValueComplexPropertyMapping OnFormatted(Func<IColumnContext, string, string> handler);
    }

    internal sealed class SeparatedValueComplexPropertyMapping<TEntity> : ISeparatedValueComplexPropertyMapping, IMemberMapping
    {
        private readonly ISeparatedValueTypeMapper<TEntity> mapper;
        private string columnName;
        private SeparatedValueOptions options;
        private INullFormatter nullFormatter;
        private IDefaultValue defaultValue;
        private Func<string, string> preprocessor;
        private Func<IColumnContext, string, string> onParsing;
        private Func<IColumnContext, object, object> onParsed;
        private Func<IColumnContext, object, object> onFormatting;
        private Func<IColumnContext, string, string> onFormatted;

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
                SeparatedValueComplexColumn column = new SeparatedValueComplexColumn(columnName, schema)
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
                return new ComplexMapperColumn<TEntity>(column, recordMapper);
            }
        }

        public IMemberAccessor Member { get; }

        public Action<IColumnContext, object, object> Reader => null;

        public Action<IColumnContext, object, object[]> Writer => null;

        public int PhysicalIndex { get; }

        public int LogicalIndex { get; }

        public ISeparatedValueComplexPropertyMapping ColumnName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(Resources.BlankColumnName);
            }
            columnName = name;
            return this;
        }

        public ISeparatedValueComplexPropertyMapping WithOptions(SeparatedValueOptions options)
        {
            this.options = options;
            return this;
        }

        public ISeparatedValueComplexPropertyMapping NullFormatter(INullFormatter formatter)
        {
            nullFormatter = formatter;
            return this;
        }

        public ISeparatedValueComplexPropertyMapping DefaultValue(IDefaultValue defaultValue)
        {
            this.defaultValue = defaultValue;
            return this;
        }

        public ISeparatedValueComplexPropertyMapping Preprocessor(Func<string, string> preprocessor)
        {
            this.preprocessor = preprocessor;
            return this;
        }

        public ISeparatedValueComplexPropertyMapping OnParsing(Func<IColumnContext, String, String> handler)
        {
            this.onParsing = handler;
            return this;
        }

        public ISeparatedValueComplexPropertyMapping OnParsed(Func<IColumnContext, object, object> handler)
        {
            this.onParsed = handler;
            return this;
        }

        public ISeparatedValueComplexPropertyMapping OnFormatting(Func<IColumnContext, object, object> handler)
        {
            this.onFormatting = handler;
            return this;
        }

        public ISeparatedValueComplexPropertyMapping OnFormatted(Func<IColumnContext, string, string> handler)
        {
            this.onFormatted = handler;
            return this;
        }
    }
}
