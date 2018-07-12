using System;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Defines a column that is part of a record schema.
    /// </summary>
    public interface IColumnDefinition
    {
        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        string ColumnName { get; }

        /// <summary>
        /// Gets whether the value in this column is returned as a result.
        /// </summary>
        bool IsIgnored { get; }

        /// <summary>
        /// Gets or sets the null handler instance used to interpret null values.
        /// </summary>
        INullHandler NullHandler { get; set; }

        /// <summary>
        /// Gets or sets a function used to preprocess input before trying to parse it.
        /// </summary>
        Func<string, string> Preprocessor { get; set; }

        /// <summary>
        /// Gets the type of the values in the column.
        /// </summary>
        Type ColumnType { get; }

        /// <summary>
        /// Gets whether nulls are allowed for the column.
        /// </summary>
        bool IsNullable { get; }

        /// <summary>
        /// Parses the given value and returns the parsed object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed value.</returns>
        object Parse(IColumnContext context, string value);

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        string Format(IColumnContext context, object value);
    }

    /// <summary>
    /// Defines a column that is part of a record schema.
    /// </summary>
    public abstract class ColumnDefinition : IColumnDefinition
    {
        private string columnName;
        private INullHandler nullHandler;

        /// <summary>
        /// Initializes a new instance of a ColumnDefinition.
        /// </summary>
        /// <param name="columnName">The name of the column to define.</param>
        protected ColumnDefinition(string columnName)
            : this(columnName, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of a ColumnDefinition.
        /// </summary>
        /// <param name="columnName">The name of the column to define.</param>
        /// <param name="isIgnored">Specifies whether the value in the column appears in the parsed record.</param>
        internal ColumnDefinition(string columnName, bool isIgnored)
        {
            IsIgnored = isIgnored;
            ColumnName = columnName;
            nullHandler = FlatFiles.NullHandler.Default;
        }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        public string ColumnName
        {
            get => columnName;
            internal set 
            {
                value = value?.Trim();
                if (!IsIgnored && String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException(Resources.BlankColumnName);
                }
                columnName = value;
            }
        }

        /// <summary>
        /// Gets whether the value in this column is returned as a result.
        /// </summary>
        public bool IsIgnored { get; }

        /// <summary>
        /// Gets or sets the null handler instance used to interpret null values.
        /// </summary>
        public INullHandler NullHandler
        {
            get => nullHandler;
            set => nullHandler = value ?? FlatFiles.NullHandler.Default;
        }

        /// <summary>
        /// Gets or sets a function used to preprocess input before trying to parse it.
        /// </summary>
        public Func<string, string> Preprocessor { get; set; }

        /// <summary>
        /// Gets the type of the values in the column.
        /// </summary>
        public abstract Type ColumnType { get; }

        /// <summary>
        /// Gets or sets whether nulls are allowed for the column.
        /// </summary>
        public bool IsNullable { get; set; } = true;

        /// <summary>
        /// Parses the given value and returns the parsed object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed value.</returns>
        public abstract object Parse(IColumnContext context, string value);

        /// <summary>
        /// Removes any leading or trailing whitespace from the value.
        /// </summary>
        /// <param name="value">The value to trim.</param>
        /// <returns>The trimmed value.</returns>
        protected internal static string TrimValue(string value)
        {
            return value?.Trim();
        }

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        public abstract string Format(IColumnContext context, object value);
    }

    /// <summary>
    /// Represents the command base class for defining custom column definitions for a class.
    /// </summary>
    /// <typeparam name="T">The type of the column.</typeparam>
    public abstract class ColumnDefinition<T> : ColumnDefinition
    {
        /// <summary>
        /// Initializes a new instance of a ColumnDefinition.
        /// </summary>
        /// <param name="columnName">The name of the column to define.</param>
        protected ColumnDefinition(string columnName) 
            : base(columnName)
        {
        }

        /// <summary>
        /// Gets the type of the values in the column.
        /// </summary>
        public override Type ColumnType => typeof(T);

        /// <summary>
        /// Parses the given value and returns the parsed object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed value.</returns>
        public override object Parse(IColumnContext context, string value)
        {
            if (Preprocessor != null)
            {
                value = Preprocessor(value);
            }
            if (NullHandler.IsNullRepresentation(context, value))
            {
                return IsNullable ? null : NullHandler.GetNullSubstitute(context);
            }
            string trimmed = IsTrimmed ? TrimValue(value) : value;
            return OnParse(context, trimmed);
        }

        /// <summary>
        /// Gets whether the value should be trimmed prior to parsing.
        /// </summary>
        protected virtual bool IsTrimmed => true;

        /// <summary>
        /// Parses the given value and returns the parsed object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed value.</returns>
        protected abstract T OnParse(IColumnContext context, string value);

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        public override string Format(IColumnContext context, object value)
        {
            if (value == null)
            {
                return NullHandler.GetNullRepresentation(context);
            }
            return OnFormat(context, (T)value);
        }

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        protected abstract string OnFormat(IColumnContext context, T value);
    }
}
