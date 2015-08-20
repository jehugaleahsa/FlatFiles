using System;
using System.Reflection;
using System.Text;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents the mapping from a type property to a byte array column.
    /// </summary>
    public interface IByteArrayPropertyMapping
    {
        /// <summary>
        /// Sets the name of the column in the input or output file.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IByteArrayPropertyMapping ColumnName(string name);

        /// <summary>
        /// Sets the encoding to use to read and write the column.
        /// </summary>
        /// <param name="encoding">The encoding to use.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IByteArrayPropertyMapping Encoding(Encoding encoding);

        /// <summary>
        /// Sets the value to treat as null.
        /// </summary>
        /// <param name="value">The value to treat as null.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IByteArrayPropertyMapping NullValue(string value);

        /// <summary>
        /// Sets a custom handler for nulls.
        /// </summary>
        /// <param name="handler">The handler to use to recognize nulls.</param>
        /// <returns>The property mapping for further configuration.</returns>
        /// <remarks>Setting the handler to null with use the default handler.</remarks>
        IByteArrayPropertyMapping NullHandler(INullHandler handler);
    }

    internal sealed class ByteArrayPropertyMapping : IByteArrayPropertyMapping, IPropertyMapping
    {
        private readonly ByteArrayColumn column;
        private readonly PropertyInfo property;

        public ByteArrayPropertyMapping(ByteArrayColumn column, PropertyInfo property)
        {
            this.column = column;
            this.property = property;
        }

        public IByteArrayPropertyMapping ColumnName(string name)
        {
            this.column.ColumnName = name;
            return this;
        }

        public IByteArrayPropertyMapping Encoding(Encoding encoding)
        {
            this.column.Encoding = encoding;
            return this;
        }

        public IByteArrayPropertyMapping NullValue(string value)
        {
            this.column.NullHandler = new ConstantNullHandler(value);
            return this;
        }

        public IByteArrayPropertyMapping NullHandler(INullHandler handler)
        {
            this.column.NullHandler = handler;
            return this;
        }

        public PropertyInfo Property
        {
            get { return property; }
        }

        public ColumnDefinition ColumnDefinition
        {
            get { return column; }
        }
    }
}
