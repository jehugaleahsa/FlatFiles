using System;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Defines the location and width of a column in a fixed-length record.
    /// </summary>
    public class Window
    {
        private readonly int width;
        private FixedAlignment alignment;

        /// <summary>
        /// Initializes a new instance of a Window.
        /// </summary>
        /// <param name="width">The maximum possible width of the column.</param>
        public Window(int width)
        {
            if (width < 0)
            {
                throw new ArgumentOutOfRangeException("width", width, Resources.InvalidColumnWidth);
            }
            this.width = width;
        }

        /// <summary>
        /// Initializes a new instance of a Window.
        /// </summary>
        /// <param name="width">The maximum possible width of the column.</param>
        /// <param name="Alignment">Alignment of the value in the column</param>
        public Window(int width, FixedAlignment Alignment) : this(width)
        {
            this.Alignment = Alignment;
        }

        /// <summary>
        /// Gets the width of the column.
        /// </summary>
        public int Width 
        { 
            get { return width; } 
        }

        /// <summary>
        /// Gets or sets the alignment of the value in the column.
        /// </summary>
        public FixedAlignment Alignment
        {
            get
            {
                return alignment;
            }
            set
            {
                if (!Enum.IsDefined(typeof(FixedAlignment), value))
                {
                    throw new ArgumentException(Resources.InvalidAlignment, "value");
                }
                alignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the character that is used fill the column.
        /// </summary>
        public char? FillCharacter { get; set; }

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
