using System;
using System.Collections.Generic;
using System.Text;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Defines the expected format of a fixed-length file record.
    /// </summary>
    public sealed class FixedLengthSchema : ISchema
    {
        private readonly SeparatedValueSchema schema;
        private readonly List<Window> windows;
        private int totalWidth;

        /// <summary>
        /// Initializes a new instance of a FixedLengthSchema.
        /// </summary>
        public FixedLengthSchema()
        {
            schema = new SeparatedValueSchema();
            windows = new List<Window>();
        }

        /// <summary>
        /// Adds a column to the schema, using the given definition to define it.
        /// </summary>
        /// <param name="definition">The definition of the column to add.</param>
        /// <param name="window">Describes the column</param>
        /// <returns>The current schema.</returns>
        public FixedLengthSchema AddColumn(ColumnDefinition definition, Window window)
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }
            schema.AddColumn(definition);
            windows.Add(window);
            totalWidth += window.Width;
            return this;
        }

        /// <summary>
        /// Gets the column definitions that make up the schema.
        /// </summary>
        public ColumnCollection ColumnDefinitions
        {
            get { return schema.ColumnDefinitions; }
        }

        /// <summary>
        /// Gets the column widths.
        /// </summary>
        public WindowCollection Windows
        {
            get { return new WindowCollection(windows); }
        }

        /// <summary>
        /// Gets the index of the column with the given name.
        /// </summary>
        /// <param name="name">The name of the column to get the index for.</param>
        /// <returns>The index of the column with the given name.</returns>
        public int GetOrdinal(string name)
        {
            return schema.GetOrdinal(name);
        }

        /// <summary>
        /// Gets the total width of all columns.
        /// </summary>
        internal int TotalWidth
        {
            get { return totalWidth; }
        }

        /// <summary>
        /// Parses the given values assuming that they are in the same order as the column definitions.
        /// </summary>
        /// <param name="values">The values to parse.</param>
        /// <returns>The parsed objects.</returns>
        internal object[] ParseValues(string[] values)
        {
            return schema.ParseValues(values);
        }

        /// <summary>
        /// Formats the given values assuming that they are in the same order as the column definitions.
        /// </summary>
        /// <param name="values">The values to format.</param>
        /// <returns>The formatted values.</returns>
        internal string[] FormatValues(object[] values)
        {
            return schema.FormatValues(values);
        }
    }
}
