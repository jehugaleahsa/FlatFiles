using System;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Defines the location and width of a column in a fixed-length record.
    /// </summary>
    public class Window
    {
        private FixedAlignment? alignment;
        private OverflowTruncationPolicy? truncationPolicy;

        /// <summary>
        /// Initializes a new instance of a Window.
        /// </summary>
        /// <param name="width">The maximum possible width of the column.</param>
        public Window(int width)
        {
            if (width < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), width, Resources.InvalidColumnWidth);
            }
            this.Width = width;
        }

        /// <summary>
        /// Gets the width of the column.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets or sets the alignment of the value in the column, using the value found in the FixedLengthOptions object by default.
        /// </summary>
        public FixedAlignment? Alignment
        {
            get
            {
                return alignment;
            }
            set
            {
                if (value != null && !Enum.IsDefined(typeof(FixedAlignment), value.Value))
                {
                    throw new ArgumentException(Resources.InvalidAlignment, nameof(value));
                }
                alignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the character that is used fill the column.
        /// </summary>
        public char? FillCharacter { get; set; }

        /// <summary>
        /// Gets or sets the truncation policy in case there is an overflow.
        /// A null policy specifies that the default truncation policy of the writer will be used.
        /// </summary>
        public OverflowTruncationPolicy? TruncationPolicy 
        {
            get
            {
                return truncationPolicy;
            }
            set
            {
                if (value != null && !Enum.IsDefined(typeof(OverflowTruncationPolicy), value.Value))
                {
                    throw new ArgumentException(Resources.InvalidTruncationPolicy, nameof(value));
                }
                else
                {
                    truncationPolicy = value;
                }
            }
        }

        /// <summary>
        /// Implicitly creates a window from a width.
        /// </summary>
        /// <param name="width">The width to create a window for.</param>
        /// <returns>The new window.</returns>
        public static implicit operator Window(int width)
        {
            return new Window(width);
        }
    }
}
