using System;
using System.Collections.Generic;

namespace FlatFiles
{
    /// <summary>
    /// Defines the expected format of a fixed-length file record.
    /// </summary>
    public sealed class FixedLengthSchema : Schema
    {
        private readonly List<Window> windows = new List<Window>();

        /// <summary>
        /// Initializes a new instance of a FixedLengthSchema.
        /// </summary>
        public FixedLengthSchema()
        {
        }
        
        /// <summary>
        /// Adds a column to the schema, using the given definition to define it.
        /// </summary>
        /// <param name="definition">The definition of the column to add.</param>
        /// <param name="window">Describes the column</param>
        /// <returns>The current schema.</returns>
        public FixedLengthSchema AddColumn(IColumnDefinition definition, Window window)
        {
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }
            AddColumnBase(definition);
            windows.Add(window);
            if (!(definition is IMetadataColumn))
            {
                TotalWidth += window.Width;
            }
            return this;
        }

        /// <summary>
        /// Gets the column widths.
        /// </summary>
        public WindowCollection Windows => new WindowCollection(windows);

        /// <summary>
        /// Gets the total width of all columns.
        /// </summary>
        internal int TotalWidth { get; private set; }
    }
}
