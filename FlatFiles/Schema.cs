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
        /// <summary>
        /// Initializes a new instance of a Schema.
        /// </summary>
        protected Schema()
        {
        }

        /// <summary>
        /// Gets the column definitions that make up the schema.
        /// </summary>
        public ColumnCollection ColumnDefinitions { get; } = new ColumnCollection();

        /// <summary>
        /// Gets the index of the column with the given name.
        /// </summary>
        /// <param name="columnName">The name of the column to get the index for.</param>
        /// <returns>The index of the column with the given name -or- -1 if the name is not found.</returns>
        public int GetOrdinal(string columnName)
        {
            return ColumnDefinitions.GetOrdinal(columnName);
        }

        /// <summary>
        /// Adds a column to the schema, using the given definition to define it.
        /// </summary>
        /// <param name="definition">The definition of the column to add.</param>
        /// <returns>The current schema.</returns>
        protected void AddColumnBase(IColumnDefinition definition)
        {
            ColumnDefinitions.AddColumn(definition);
        }

        /// <summary>
        /// Parses the given values assuming that they are in the same order as the column definitions.
        /// </summary>
        /// <param name="context">The metadata for the current record being processed.</param>
        /// <param name="values">The values to parse.</param>
        /// <returns>The parsed objects.</returns>
        protected object[] ParseValuesBase(IRecordContext context, string[] values)
        {
            object[] parsedValues = new object[ColumnDefinitions.PhysicalCount];
            for (int columnIndex = 0, sourceIndex = 0, destinationIndex = 0; columnIndex != ColumnDefinitions.Count; ++columnIndex)
            {
                var definition = ColumnDefinitions[columnIndex];
                if (definition is IMetadataColumn)
                {
                    var columnContext = GetColumnContext(context, columnIndex, destinationIndex);
                    var metadata = Parse(columnContext, definition, null);
                    parsedValues[destinationIndex] = metadata;
                    ++destinationIndex;
                }
                else if (!definition.IsIgnored)
                {
                    var columnContext = GetColumnContext(context, columnIndex, destinationIndex);
                    var rawValue = values[sourceIndex];
                    var parsedValue = Parse(columnContext, definition, rawValue);
                    parsedValues[destinationIndex] = parsedValue;
                    ++sourceIndex;
                    ++destinationIndex;
                }
                else
                {
                    ++sourceIndex;
                }
            }
            return parsedValues;
        }

        private object Parse(IColumnContext columnContext, IColumnDefinition definition, string rawValue)
        {
            try
            {
                object parsedValue = definition.Parse(columnContext, rawValue);
                return parsedValue;
            }
            catch (Exception exception)
            {
                throw new ColumnProcessingException(columnContext, rawValue, exception);
            }
        }

        /// <summary>
        /// Formats the given values assuming that they are in the same order as the column definitions.
        /// </summary>
        /// <param name="context">The metadata for the record currently being processed.</param>
        /// <param name="values">The values to format.</param>
        /// <returns>The formatted values.</returns>
        protected string[] FormatValuesBase(IRecordContext context, object[] values)
        {
            string[] formattedValues = new string[ColumnDefinitions.Count];
            for (int columnIndex = 0, valueIndex = 0; columnIndex != ColumnDefinitions.Count; ++columnIndex)
            {
                IColumnDefinition definition = ColumnDefinitions[columnIndex];
                if (definition is IMetadataColumn)
                {
                    var columnContext = GetColumnContext(context, columnIndex, valueIndex);
                    var formattedValue = Format(columnContext, definition, null);
                    formattedValues[columnIndex] = formattedValue;
                    ++valueIndex;
                }
                else if (!definition.IsIgnored)
                {
                    var columnContext = GetColumnContext(context, columnIndex, valueIndex);
                    var value = values[valueIndex];
                    string formattedValue = Format(columnContext, definition, value);
                    formattedValues[columnIndex] = formattedValue;
                    ++valueIndex;
                }
            }
            return formattedValues;
        }

        private static string Format(IColumnContext columnContext, IColumnDefinition definition, object value)
        {
            try
            {
                return definition.Format(columnContext, value);
            }
            catch (Exception exception)
            {
                throw new ColumnProcessingException(columnContext, value, exception);
            }
        }

        private ColumnContext GetColumnContext(IRecordContext context, int physicalIndex, int logicalIndex)
        {
            return new ColumnContext()
            {
                RecordContext = context,
                PhysicalIndex = physicalIndex,
                LogicalIndex = logicalIndex
            };
        }
    }
}
