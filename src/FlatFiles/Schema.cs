using System;
using System.Linq;
using FlatFiles.Resources;

namespace FlatFiles
{
    /// <summary>
    /// Defines the expected format of a record in a file.
    /// </summary>
    public interface ISchema
    {
        /// <summary>
        /// Gets the column definitions that make up the schema.
        /// </summary>
        ColumnCollection ColumnDefinitions { get; }

        /// <summary>
        /// Gets the index of the column with the given name.
        /// </summary>
        /// <param name="columnName">The name of the column to get the index for.</param>
        /// <returns>The index of the column with the given name -or- -1 if the name is not found.</returns>
        int GetOrdinal(string columnName);
    }

    /// <summary>
    /// Defines the expected format of a record in a file.
    /// </summary>
    public abstract class Schema : ISchema
    {
        private readonly ColumnCollection definitions;

        /// <summary>
        /// Initializes a new instance of a Schema.
        /// </summary>
        protected Schema()
        {
            this.definitions = new ColumnCollection();
        }

        /// <summary>
        /// Gets the column definitions that make up the schema.
        /// </summary>
        public ColumnCollection ColumnDefinitions
        {
            get { return definitions; }
        }

        /// <summary>
        /// Gets the index of the column with the given name.
        /// </summary>
        /// <param name="columnName">The name of the column to get the index for.</param>
        /// <returns>The index of the column with the given name -or- -1 if the name is not found.</returns>
        public int GetOrdinal(string columnName)
        {
            return definitions.GetOrdinal(columnName);
        }

        /// <summary>
        /// Adds a column to the schema, using the given definition to define it.
        /// </summary>
        /// <param name="definition">The definition of the column to add.</param>
        /// <returns>The current schema.</returns>
        protected void AddColumnBase(IColumnDefinition definition)
        {
            definitions.AddColumn(definition);
        }

        /// <summary>
        /// Parses the given values assuming that they are in the same order as the column definitions.
        /// </summary>
        /// <param name="values">The values to parse.</param>
        /// <returns>The parsed objects.</returns>
        protected object[] ParseValuesBase(string[] values)
        {
            var parsedValues = from definition in definitions
                               where !definition.IsIgnored
                               select parse(definition, values);
            return parsedValues.ToArray();
        }

        private object parse(IColumnDefinition definition, string[] values)
        {
            int position = definitions.GetOrdinal(definition.ColumnName);
            string rawValue = values[position];
            try
            {
                object parsedValue = definition.Parse(rawValue);
                return parsedValue;
            }
            catch (Exception exception)
            {
                string message = String.Format(null, SharedResources.InvalidColumnConversion, rawValue, definition.ColumnType.FullName, definition.ColumnName, position);
                throw new FlatFileException(message, exception);
            }
        }

        /// <summary>
        /// Formats the given values assuming that they are in the same order as the column definitions.
        /// </summary>
        /// <param name="values">The values to format.</param>
        /// <returns>The formatted values.</returns>
        protected string[] FormatValuesBase(object[] values)
        {
            string[] formattedValues = new string[definitions.Count];
            int index = 0;
            foreach (IColumnDefinition definition in definitions.Where(d => !d.IsIgnored))
            {
                int position = definitions.GetOrdinal(definition.ColumnName);
                object value = values[index];
                string formattedValue = definition.Format(value);
                formattedValues[position] = formattedValue;
                ++index;
            }
            return formattedValues;
        }
    }
}
