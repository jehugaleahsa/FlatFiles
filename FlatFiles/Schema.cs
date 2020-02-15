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
        public virtual ColumnCollection ColumnDefinitions { get; } = new ColumnCollection();

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
        internal object[] ParseValues(IRecoverableRecordContext context, string[] values)
        {
            object[] parsedValues = new object[ColumnDefinitions.PhysicalCount];
            for (int columnIndex = 0, sourceIndex = 0, destinationIndex = 0; columnIndex != ColumnDefinitions.Count; ++columnIndex)
            {
                var definition = ColumnDefinitions[columnIndex];
                if (definition is IMetadataColumn)
                {
                    var columnContext = GetColumnContext(context, columnIndex, destinationIndex);
                    var metadata = ParseWithContext(columnContext, null);
                    parsedValues[destinationIndex] = metadata;
                    ++destinationIndex;
                }
                else if (!definition.IsIgnored)
                {
                    var rawValue = values[sourceIndex];
                    var parsedValue = ParseValue(context, columnIndex, destinationIndex, rawValue);
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

        private object ParseValue(IRecoverableRecordContext context, int columnIndex, int destinationIndex, string rawValue)
        {
            var isContextDisabled = context.ExecutionContext.Options.IsColumnContextDisabled;
            if (isContextDisabled)
            {
                var definition = ColumnDefinitions[columnIndex];
                return ParseWithoutContext(definition, destinationIndex, rawValue);
            }
            else
            {
                var columnContext = GetColumnContext(context, columnIndex, destinationIndex);
                return ParseWithContext(columnContext, rawValue);
            }
        }

        private object ParseWithContext(IColumnContext columnContext, string rawValue)
        {
            try
            {
                var definition = columnContext.ColumnDefinition;
                object parsedValue = definition.Parse(columnContext, rawValue);
                return parsedValue;
            }
            catch (Exception exception)
            {
                var columnException = new ColumnProcessingException(columnContext, rawValue, exception);
                if (columnContext.RecordContext is IRecoverableRecordContext recordContext && recordContext.HasHandler)
                {
                    var e = new ColumnErrorEventArgs(columnException);
                    recordContext.ProcessError(this, e);
                    if (e.IsHandled)
                    {
                        return e.Substitution;
                    }
                }
                throw columnException;
            }
        }

        private static object ParseWithoutContext(IColumnDefinition definition, int position, string rawValue)
        {
            try
            {
                object parsedValue = definition.Parse(null, rawValue);
                return parsedValue;
            }
            catch (Exception exception)
            {
                throw new ColumnProcessingException(definition, position, rawValue, exception);
            }
        }

        /// <summary>
        /// Formats the given values assuming that they are in the same order as the column definitions.
        /// </summary>
        /// <param name="context">The metadata for the record currently being processed.</param>
        /// <param name="values">The values to format.</param>
        /// <returns>The formatted values.</returns>
        internal string[] FormatValues(IRecoverableRecordContext context, object[] values)
        {
            string[] formattedValues = new string[ColumnDefinitions.Count];
            for (int columnIndex = 0, valueIndex = 0; columnIndex != ColumnDefinitions.Count; ++columnIndex)
            {
                IColumnDefinition definition = ColumnDefinitions[columnIndex];
                if (definition is IMetadataColumn)
                {
                    var columnContext = GetColumnContext(context, columnIndex, valueIndex);
                    var formattedValue = FormatWithContext(columnContext, null);
                    formattedValues[columnIndex] = formattedValue;
                    ++valueIndex;
                }
                else if (!definition.IsIgnored)
                {
                    var value = values[valueIndex];
                    var formattedValue = FormatValue(context, columnIndex, valueIndex, value);
                    formattedValues[columnIndex] = formattedValue;
                    ++valueIndex;
                }
            }
            return formattedValues;
        }

        private string FormatValue(IRecoverableRecordContext context, int columnIndex, int valueIndex, object value)
        {
            var isContextDisabled = context.ExecutionContext.Options.IsColumnContextDisabled;
            if (isContextDisabled)
            {
                var definition = ColumnDefinitions[columnIndex];
                return FormatWithoutContext(definition, valueIndex, value);
            }
            else
            {
                var columnContext = GetColumnContext(context, columnIndex, valueIndex);
                return FormatWithContext(columnContext, value);
            }
        }

        private string FormatWithContext(IColumnContext columnContext, object value)
        {
            try
            {
                var definition = columnContext.ColumnDefinition;
                return definition.Format(columnContext, value);
            }
            catch (Exception exception)
            {
                var columnException = new ColumnProcessingException(columnContext, value, exception);
                if (columnContext.RecordContext is IRecoverableRecordContext recordContext && recordContext.HasHandler)
                {
                    var e = new ColumnErrorEventArgs(columnException);
                    recordContext.ProcessError(this, e);
                    if (e.IsHandled)
                    {
                        return (string)e.Substitution;
                    }
                }
                throw columnException;
            }
        }

        private static string FormatWithoutContext(IColumnDefinition definition, int position, object value)
        {
            try
            {
                return definition.Format(null, value);
            }
            catch (Exception exception)
            {
                throw new ColumnProcessingException(definition, position, value, exception);
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
