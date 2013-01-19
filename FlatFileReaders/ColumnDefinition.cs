using System;
using FlatFileReaders.Properties;

namespace FlatFileReaders
{
    /// <summary>
    /// Defines a column that is part of a record schema.
    /// </summary>
    public abstract class ColumnDefinition
    {
        private readonly string columnName;

        /// <summary>
        /// Initializes a new instance of a ColumnDefinition.
        /// </summary>
        /// <param name="columnName">The name of the column to define.</param>
        protected ColumnDefinition(string columnName)
        {
            if (String.IsNullOrWhiteSpace(columnName))
            {
                throw new ArgumentException(Resources.BlankColumnName);
            }
            this.columnName = columnName.Trim().ToLowerInvariant();
        }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        public string ColumnName
        {
            get { return columnName; }
        }

        /// <summary>
        /// Parses the given value and returns the parsed object.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed value.</returns>
        public abstract object Parse(string value);
    }
}
