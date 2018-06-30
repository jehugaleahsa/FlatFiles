using System;

namespace FlatFiles
{
    /// <summary>
    /// Represents a column whose data is not sourced by the input file.
    /// </summary>
    public interface IMetadataColumn
    {
    }

    /// <summary>
    /// Represents a column whose data is not sourced by the input file.
    /// </summary>
    /// <typeparam name="T">The type of the metadata.</typeparam>
    public abstract class MetadataColumn<T> : ColumnDefinition, IMetadataColumn
    {
        /// <summary>
        /// Initializes a new instance of a MetadataColumn.
        /// </summary>
        /// <param name="columnName">The name of the metadata column.</param>
        protected MetadataColumn(string columnName)
            : base(columnName)
        {
        }

        /// <summary>
        /// Gets the type of the values in the column.
        /// </summary>
        public override Type ColumnType => typeof(T);

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        public override sealed string Format(IColumnContext context, object value)
        {
            return OnFormat(context);
        }

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <returns>The formatted value.</returns>
        protected abstract string OnFormat(IColumnContext context);

        /// <summary>
        /// Parses the given value and returns the parsed object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed value.</returns>
        public sealed override object Parse(IColumnContext context, string value)
        {
            return OnParse(context);
        }

        /// <summary>
        /// Parses the given value and returns the parsed object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <returns>The parsed value.</returns>
        protected abstract T OnParse(IColumnContext context);
    }
}
