using System;

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
        /// <param name="metadata">The current metadata for the process.</param>
        /// <param name="values">The values to parse.</param>
        /// <returns>The parsed objects.</returns>
        protected object[] ParseValuesBase(IProcessMetadata metadata, string[] values)
        {
            object[] parsedValues = new object[definitions.PhysicalCount];
            for (int columnIndex = 0, sourceIndex = 0, destinationIndex = 0; columnIndex != definitions.Count; ++columnIndex)
            {
                IColumnDefinition definition = definitions[columnIndex];
                if (definition is IMetadataColumn metaColumn)
                {
                    object value = metaColumn.GetValue(metadata);
                    parsedValues[destinationIndex] = value;
                    ++destinationIndex;
                }
                else if (!definition.IsIgnored)
                {
                    string rawValue = values[sourceIndex];
                    ++sourceIndex;
                    object parsedValue = parse(definition, rawValue);
                    parsedValues[destinationIndex] = parsedValue;
                    ++destinationIndex;
                }
                else
                {
                    ++sourceIndex;
                }
            }
            return parsedValues;
        }

        private object parse(IColumnDefinition definition, string rawValue)
        {
            try
            {
                object parsedValue = definition.Parse(rawValue);
                return parsedValue;
            }
            catch (Exception exception)
            {
                throw new ColumnProcessingException(this, definition, rawValue, exception);
            }
        }

        /// <summary>
        /// Formats the given values assuming that they are in the same order as the column definitions.
        /// </summary>
        /// <param name="metadata">The current metadata for the process.</param>
        /// <param name="values">The values to format.</param>
        /// <returns>The formatted values.</returns>
        protected string[] FormatValuesBase(IProcessMetadata metadata, object[] values)
        {
            string[] formattedValues = new string[definitions.Count];
            for (int columnIndex = 0, valueIndex = 0; columnIndex != definitions.Count; ++columnIndex)
            {
                IColumnDefinition definition = definitions[columnIndex];
                if (definition is IMetadataColumn metaColumn)
                {
                    object value = metaColumn.GetValue(metadata);
                    string formattedValue = definition.Format(value);
                    formattedValues[columnIndex] = formattedValue;
                    ++valueIndex;
                }
                else if (!definition.IsIgnored)
                {
                    object value = values[valueIndex];
                    string formattedValue = definition.Format(value);
                    formattedValues[columnIndex] = formattedValue;
                    ++valueIndex;
                }
            }
            return formattedValues;
        }
    }
}
