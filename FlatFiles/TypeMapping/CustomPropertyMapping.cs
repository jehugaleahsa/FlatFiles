using System;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents the mapping from a type property to a custom column.
    /// </summary>
    public interface ICustomPropertyMapping
    {
        /// <summary>
        /// Sets the value to treat as null.
        /// </summary>
        /// <param name="value">The value to treat as null.</param>
        /// <returns>The property mapping for further configuration.</returns>
        ICustomPropertyMapping NullValue(string value);

        /// <summary>
        /// Sets a custom handler for nulls.
        /// </summary>
        /// <param name="handler">The handler to use to recognize nulls.</param>
        /// <returns>The property mapping for further configuration.</returns>
        /// <remarks>Setting the handler to null with use the default handler.</remarks>
        ICustomPropertyMapping NullHandler(INullHandler handler);

        /// <summary>
        /// Sets a function to preprocess in the input before parsing it.
        /// </summary>
        /// <param name="preprocessor">A preprocessor function.</param>
        /// <returns>The property mapping for further configuration.</returns>
        ICustomPropertyMapping Preprocessor(Func<string, string> preprocessor);
    }

    internal sealed class CustomPropertyMapping : ICustomPropertyMapping, IMemberMapping
    {
        public CustomPropertyMapping(IColumnDefinition column, IMemberAccessor member, int physicalIndex, int logicalIndex)
        {
            ColumnDefinition = column;
            Member = member;
            PhysicalIndex = physicalIndex;
            LogicalIndex = logicalIndex;
        }

        public ICustomPropertyMapping NullValue(string value)
        {
            ColumnDefinition.NullHandler = new ConstantNullHandler(value);
            return this;
        }

        public ICustomPropertyMapping NullHandler(INullHandler handler)
        {
            ColumnDefinition.NullHandler = handler;
            return this;
        }

        public ICustomPropertyMapping Preprocessor(Func<string, string> preprocessor)
        {
            ColumnDefinition.Preprocessor = preprocessor;
            return this;
        }

        public IMemberAccessor Member { get; }

        public IColumnDefinition ColumnDefinition { get; }

        public int PhysicalIndex { get; }

        public int LogicalIndex { get; }
    }
}
