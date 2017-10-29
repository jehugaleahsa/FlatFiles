using System;
using System.Reflection;
using FlatFiles.Resources;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents the mapping from a type property to an object.
    /// </summary>
    public interface IFixedLengthComplexPropertyMapping
    {
        /// <summary>
        /// Sets the name of the column in the input or output file.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IFixedLengthComplexPropertyMapping ColumnName(string name);

        /// <summary>
        /// Sets the options to use when reading/writing the complex type.
        /// </summary>
        /// <param name="options">The options to use.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IFixedLengthComplexPropertyMapping WithOptions(FixedLengthOptions options);

        /// <summary>
        /// Sets the value to treat as null.
        /// </summary>
        /// <param name="value">The value to treat as null.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IFixedLengthComplexPropertyMapping NullValue(string value);

        /// <summary>
        /// Sets a custom handler for nulls.
        /// </summary>
        /// <param name="handler">The handler to use to recognize nulls.</param>
        /// <returns>The property mapping for further configuration.</returns>
        /// <remarks>Setting the handler to null with use the default handler.</remarks>
        IFixedLengthComplexPropertyMapping NullHandler(INullHandler handler);

        /// <summary>
        /// Sets a function to preprocess in the input before parsing it.
        /// </summary>
        /// <param name="preprocessor">A preprocessor function.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IFixedLengthComplexPropertyMapping Preprocessor(Func<string, string> preprocessor);
    }

    internal sealed class FixedLengthComplexPropertyMapping<TEntity> : IFixedLengthComplexPropertyMapping, IMemberMapping
    {
        private readonly IFixedLengthTypeMapper<TEntity> mapper;
        private readonly IMemberAccessor member;
        private string columnName;
        private FixedLengthOptions options;
        private INullHandler nullHandler;
        private Func<string, string> preprocessor;

        public FixedLengthComplexPropertyMapping(IFixedLengthTypeMapper<TEntity> mapper, IMemberAccessor member)
        {
            this.mapper = mapper;
            this.member = member;
            this.columnName = member.Name;
        }

        public IColumnDefinition ColumnDefinition
        {
            get
            {
                FixedLengthSchema schema = mapper.GetSchema();
                FixedLengthComplexColumn column = new FixedLengthComplexColumn(columnName, schema);
                column.Options = options;
                column.NullHandler = nullHandler;
                column.Preprocessor = preprocessor;

                var recordMapper = (IRecordMapper<TEntity>)mapper;
                return new ComplexMapperColumn<TEntity>(column, recordMapper);
            }
        }

        public IMemberAccessor Member
        {
            get { return member; }
        }

        public IFixedLengthComplexPropertyMapping ColumnName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(SharedResources.BlankColumnName);
            }
            this.columnName = name;
            return this;
        }

        public IFixedLengthComplexPropertyMapping WithOptions(FixedLengthOptions options)
        {
            this.options = options;
            return this;
        }

        public IFixedLengthComplexPropertyMapping NullHandler(INullHandler handler)
        {
            this.nullHandler = handler;
            return this;
        }

        public IFixedLengthComplexPropertyMapping NullValue(string value)
        {
            this.nullHandler = new ConstantNullHandler(value);
            return this;
        }

        public IFixedLengthComplexPropertyMapping Preprocessor(Func<string, string> preprocessor)
        {
            this.preprocessor = preprocessor;
            return this;
        }
    }
}
