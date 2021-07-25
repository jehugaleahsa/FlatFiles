using System;
using System.Collections.Generic;

namespace FlatFiles
{
    /// <summary>
    /// Defines the expected format of a fixed-length file record.
    /// </summary>
    public sealed class FixedLengthSchema : Schema
    {
        private readonly List<Window> windows = new();
        private ColumnCollection? cachedColumns;
        private IColumnDefinition? trailing;

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
            if (window == Window.Trailing)
            {
                trailing = definition;
            }
            else
            {
                AddColumnBase(definition);
                windows.Add(window);
                if (!(definition is IMetadataColumn))
                {
                    TotalWidth += window.Width;
                }
            }
            cachedColumns = null;
            return this;
        }

        /// <inheritdoc />
        public override ColumnCollection ColumnDefinitions
        {
            get
            {
                if (trailing == null)
                {
                    return base.ColumnDefinitions;
                }
                else if (cachedColumns == null)
                {
                    var copy = new ColumnCollection(base.ColumnDefinitions);
                    copy.AddColumn(trailing);
                    this.cachedColumns = copy;
                    return copy;
                }
                else
                {
                    return cachedColumns;
                }
            }
        }

        /// <summary>
        /// Gets the column widths.
        /// </summary>
        public WindowCollection Windows => new(windows);

        /// <summary>
        /// Gets the total width of all columns.
        /// </summary>
        internal int TotalWidth { get; private set; }
    }
}
