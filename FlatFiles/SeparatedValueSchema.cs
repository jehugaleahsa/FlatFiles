using System;

namespace FlatFiles
{
    /// <summary>
    /// Defines the expected format of a record in a file.
    /// </summary>
    public sealed class SeparatedValueSchema : Schema
    {
        /// <summary>
        /// Initializes a new instance of a Schema.
        /// </summary>
        public SeparatedValueSchema()
        {
        }

        /// <summary>
        /// Adds a column to the schema, using the given definition to define it.
        /// </summary>
        /// <param name="definition">The definition of the column to add.</param>
        /// <returns>The current schema.</returns>
        public SeparatedValueSchema AddColumn(IColumnDefinition definition)
        {
            AddColumnBase(definition);
            return this;
        }

        /// <summary>
        /// Parses the given values assuming that they are in the same order as the column definitions.
        /// </summary>
        /// <param name="metadata">The current metadata for the process.</param>
        /// <param name="values">The values to parse.</param>
        /// <returns>The parsed objects.</returns>
        internal object[] ParseValues(IProcessMetadata metadata, string[] values)
        {
            return ParseValuesBase(metadata, values);
        }

        /// <summary>
        /// Formats the given values assuming that they are in the same order as the column definitions.
        /// </summary>
        /// <param name="metadata">The current metadata for the process.</param>
        /// <param name="values">The values to format.</param>
        /// <returns>The formatted values.</returns>
        internal string[] FormatValues(IProcessMetadata metadata, object[] values)
        {
            return FormatValuesBase(metadata, values);
        }
    }
}
