using System;
using System.Globalization;
using System.Reflection;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents the mapping from a type property to a decimal column.
    /// </summary>
    public interface IDecimalPropertyMapping
    {
        /// <summary>
        /// Sets the name of the column in the input or output file.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IDecimalPropertyMapping ColumnName(string name);

        /// <summary>
        /// Sets the format provider to use.
        /// </summary>
        /// <param name="provider">The provider to use.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IDecimalPropertyMapping FormatProvider(IFormatProvider provider);

        /// <summary>
        /// Sets the number styles to expect when parsing the input.
        /// </summary>
        /// <param name="styles">The number styles used in the input.</param>
        /// <returns>The property mappig for further configuration.</returns>
        IDecimalPropertyMapping NumberStyles(NumberStyles styles);

        /// <summary>
        /// Sets the output format to use.
        /// </summary>
        /// <param name="format">The format to use.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IDecimalPropertyMapping OutputFormat(string format);

        /// <summary>
        /// Sets the value to treat as null.
        /// </summary>
        /// <param name="value">The value to treat as null.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IDecimalPropertyMapping NullValue(string value);

        /// <summary>
        /// Sets a custom handler for nulls.
        /// </summary>
        /// <param name="handler">The handler to use to recognize nulls.</param>
        /// <returns>The property mapping for further configuration.</returns>
        /// <remarks>Setting the handler to null with use the default handler.</remarks>
        IDecimalPropertyMapping NullHandler(INullHandler handler);
    }

    internal sealed class DecimalPropertyMapping : IDecimalPropertyMapping, IPropertyMapping
    {
        private readonly DecimalColumn column;
        private readonly PropertyInfo property;

        public DecimalPropertyMapping(DecimalColumn column, PropertyInfo property)
        {
            this.column = column;
            this.property = property;
        }

        public IDecimalPropertyMapping ColumnName(string name)
        {
            this.column.ColumnName = name;
            return this;
        }

        public IDecimalPropertyMapping FormatProvider(IFormatProvider provider)
        {
            this.column.FormatProvider = provider;
            return this;
        }

        public IDecimalPropertyMapping NumberStyles(NumberStyles styles)
        {
            this.column.NumberStyles = styles;
            return this;
        }

        public IDecimalPropertyMapping OutputFormat(string format)
        {
            this.column.OutputFormat = format;
            return this;
        }

        public IDecimalPropertyMapping NullValue(string value)
        {
            this.column.NullHandler = new ConstantNullHandler(value);
            return this;
        }

        public IDecimalPropertyMapping NullHandler(INullHandler handler)
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
