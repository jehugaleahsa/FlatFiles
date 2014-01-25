using System;
using System.Collections.Generic;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Defines the expected format of a fixed-length file record.
    /// </summary>
    public sealed class FixedLengthSchema : ISchema
    {
        private readonly SeparatedValueSchema schema;
        private readonly List<int> widths;
        private readonly List<FixedAlignment> alignments;
        private int totalWidth;

        /// <summary>
        /// Initializes a new instance of a FixedLengthSchema.
        /// </summary>
        public FixedLengthSchema()
        {
            schema = new SeparatedValueSchema();
            widths = new List<int>();
            alignments = new List<FixedAlignment>();
        }

        /// <summary>
        /// Adds a column to the schema, using the given definition to define it.
        /// </summary>
        /// <param name="definition">The definition of the column to add.</param>
        /// <param name="width">The number of characters used by the column in the file.</param>
        /// <param name="alignment">The alignment of the value in the column.</param>
        /// <returns>The current schema.</returns>
        public FixedLengthSchema AddColumn(
            ColumnDefinition definition, 
            int width, 
            FixedAlignment alignment = FixedAlignment.LeftAligned)
        {
            if (width < 0)
            {
                throw new ArgumentOutOfRangeException("width", width, Resources.InvalidColumnWidth);
            }
            if (!Enum.IsDefined(typeof(FixedAlignment), alignment))
            {
                throw new ArgumentException(Resources.InvalidAlignment, "alignment");
            }
            schema.AddColumn(definition);
            widths.Add(width);
            alignments.Add(alignment);
            totalWidth += width;
            return this;
        }

        /// <summary>
        /// Adds a column to the schema with the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the column.</typeparam>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="width">The number of characters used by the column in the file.</param>
        /// <param name="alignment">The alignment of the value in the column.</param>
        /// <returns>The current schema.</returns>
        public FixedLengthSchema AddColumn<T>(
            string columnName, 
            int width, 
            FixedAlignment alignment = FixedAlignment.LeftAligned)
        {
            if (width < 0)
            {
                throw new ArgumentOutOfRangeException("width", width, Resources.InvalidColumnWidth);
            }
            if (!Enum.IsDefined(typeof(FixedAlignment), alignment))
            {
                throw new ArgumentException(Resources.InvalidAlignment, "alignment");
            }
            schema.AddColumn<T>(columnName);
            widths.Add(width);
            alignments.Add(alignment);
            totalWidth += width;
            return this;
        }

        /// <summary>
        /// Adds a column to the schema, using the given definition to define it.
        /// </summary>
        /// <typeparam name="T">The type of the column.</typeparam>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="width">The number of characters used by the column in the file.</param>
        /// <param name="alignment">The alignment of the value in the column.</param>
        /// <param name="parser">A function that converts the parsed string value to the appropriate type.</param>
        /// <param name="formatter">A function that convert the value to a string.</param>
        /// <returns>The current schema.</returns>
        public FixedLengthSchema AddColumn<T>(
            string columnName, 
            int width, 
            FixedAlignment alignment = FixedAlignment.LeftAligned, 
            Func<string, T> parser = null, 
            Func<T, string> formatter = null)
        {
            if (width < 0)
            {
                throw new ArgumentOutOfRangeException("width", width, Resources.InvalidColumnWidth);
            }
            if (!Enum.IsDefined(typeof(FixedAlignment), alignment))
            {
                throw new ArgumentException(Resources.InvalidAlignment, "alignment");
            }
            schema.AddColumn<T>(columnName, parser, formatter);
            widths.Add(width);
            alignments.Add(alignment);
            totalWidth += width;
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
        internal List<int> ColumnWidths
        {
            get { return widths; }
        }

        /// <summary>
        /// Gets the column alignments.
        /// </summary>
        internal List<FixedAlignment> ColumnAlignments
        {
            get { return alignments; }
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
