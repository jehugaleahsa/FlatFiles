using System;
using System.Reflection;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents the mapping from a type property to a DateTime column.
    /// </summary>
    public interface IDateTimePropertyMapping
    {
        /// <summary>
        /// Sets the name of the column in the input or output file.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IDateTimePropertyMapping ColumnName(string name);

        /// <summary>
        /// Sets the date/time format the input is expected to be in.
        /// </summary>
        /// <param name="format">The format to expect.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IDateTimePropertyMapping InputFormat(string format);

        /// <summary>
        /// Sets the date/time format to use for output.
        /// </summary>
        /// <param name="format">The format to use.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IDateTimePropertyMapping OutputFormat(string format);

        /// <summary>
        /// Sets the format provider to use when reading and writing date/times.
        /// </summary>
        /// <param name="provider">The provider to use.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IDateTimePropertyMapping FormatProvider(IFormatProvider provider);

        /// <summary>
        /// Sets the value to treat as null.
        /// </summary>
        /// <param name="value">The value to treat as null.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IDateTimePropertyMapping NullValue(string value);

        /// <summary>
        /// Sets a custom handler for nulls.
        /// </summary>
        /// <param name="handler">The handler to use to recognize nulls.</param>
        /// <returns>The property mapping for further configuration.</returns>
        /// <remarks>Setting the handler to null with use the default handler.</remarks>
        IDateTimePropertyMapping NullHandler(INullHandler handler);
    }

    internal sealed class DateTimePropertyMapping : IDateTimePropertyMapping, IPropertyMapping
    {
        private readonly DateTimeColumn column;
        private readonly PropertyInfo property;

        public DateTimePropertyMapping(DateTimeColumn column, PropertyInfo property)
        {
            this.column = column;
            this.property = property;
        }

        public IDateTimePropertyMapping ColumnName(string name)
        {
            this.column.ColumnName = name;
            return this;
        }

        public IDateTimePropertyMapping InputFormat(string format)
        {
            this.column.InputFormat = format;
            return this;
        }

        public IDateTimePropertyMapping OutputFormat(string format)
        {
            this.column.OutputFormat = format;
            return this;
        }

        public IDateTimePropertyMapping FormatProvider(IFormatProvider provider)
        {
            this.column.FormatProvider = provider;
            return this;
        }

        public IDateTimePropertyMapping NullValue(string value)
        {
            this.column.NullHandler = new ConstantNullHandler(value);
            return this;
        }

        public IDateTimePropertyMapping NullHandler(INullHandler handler)
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
