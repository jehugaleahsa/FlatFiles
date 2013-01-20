using System;
using System.Collections.Generic;
using FlatFileReaders.Properties;

namespace FlatFileReaders
{
    /// <summary>
    /// Defines the expected format of a fixed-length file record.
    /// </summary>
    public sealed class FixedLengthSchema
    {
        private readonly Schema schema;
        private readonly List<int> widths;
        private int totalWidth;

        /// <summary>
        /// Initializes a new instance of a FixedLengthSchema.
        /// </summary>
        public FixedLengthSchema()
        {
            schema = new Schema();
            widths = new List<int>();
        }

        /// <summary>
        /// Adds a column to the schema, using the given definition to define it.
        /// </summary>
        /// <param name="definition">The definition of the column to add.</param>
        /// <param name="width">The number of characters used by the column in the file.</param>
        /// <returns>The current schema.</returns>
        public FixedLengthSchema AddColumn(ColumnDefinition definition, int width)
        {
            if (width < 1)
            {
                throw new ArgumentOutOfRangeException("width", width, Resources.InvalidColumnWidth);
            }
            schema.AddColumn(definition);
            widths.Add(width);
            totalWidth += width;
            return this;
        }

        /// <summary>
        /// Gets the underlying schema.
        /// </summary>
        internal Schema Schema
        {
            get { return schema; }
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
        internal List<int> ColumnWidths
        {
            get { return widths; }
        }

        /// <summary>
        /// Gets the total width of all columns.
        /// </summary>
        internal int TotalWidth
        {
            get { return totalWidth; }
        }

        /// <summary>
        /// Parses the given values assuming that the are in the same order as the column definitions.
        /// </summary>
        /// <param name="values">The values to parse.</param>
        /// <returns>The parsed objects.</returns>
        internal object[] ParseValues(string[] values)
        {
            return schema.ParseValues(values);
        }
    }
}
