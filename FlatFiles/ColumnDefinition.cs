using System;
using System.Text;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Defines a column that is part of a record schema.
    /// </summary>
    public abstract class ColumnDefinition
    {
        private string columnName;
        private INullHandler nullHandler;

        /// <summary>
        /// Initializes a new instance of a ColumnDefinition.
        /// </summary>
        /// <param name="columnName">The name of the column to define.</param>
        protected ColumnDefinition(string columnName)
        {
            ColumnName = columnName;
            nullHandler = new DefaultNullHandler();
        }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        public string ColumnName
        {
            get 
            { 
                return columnName; 
            }
            internal set 
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException(Resources.BlankColumnName);
                }
                columnName = value;
            }
        }

        /// <summary>
        /// Gets or sets the null handler instance used to interpret null values.
        /// </summary>
        public INullHandler NullHandler
        {
            get { return nullHandler; }
            set { nullHandler = value ?? new DefaultNullHandler(); }
        }

        /// <summary>
        /// Gets the type of the values in the column.
        /// </summary>
        public abstract Type ColumnType { get; }

        /// <summary>
        /// Parses the given value and returns the parsed object.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed value.</returns>
        public abstract object Parse(string value);

        /// <summary>
        /// Removes any leading or trailing whitespace from the value.
        /// </summary>
        /// <param name="value">The value to trim.</param>
        /// <returns>The trimmed value.</returns>
        protected string TrimValue(string value)
        {
            if (value == null)
            {
                return String.Empty;
            }
            return value.Trim();
        }

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        public abstract string Format(object value);
    }
}
