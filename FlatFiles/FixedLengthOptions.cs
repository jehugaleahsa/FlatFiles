using System;
using System.Text;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Holds configuration settings for the FixedLengthParser class.
    /// </summary>
    public sealed class FixedLengthOptions : ICloneable
    {
        private OverflowTruncationPolicy truncationPolicy;

        /// <summary>
        /// Initializes a new instance of a FixedLengthParserOptions.
        /// </summary>
        public FixedLengthOptions()
        {
            FillCharacter = ' ';
            RecordSeparator = Environment.NewLine;
            truncationPolicy = OverflowTruncationPolicy.TruncateLeading;
        }

        /// <summary>
        /// Gets or sets the character used to buffer values in a column.
        /// </summary>
        public char FillCharacter { get; set; }

        /// <summary>
        /// Gets or sets the string that indicates the end of a record.
        /// </summary>
        public string RecordSeparator { get; set; }

        /// <summary>
        /// Gets or sets the encoding of the text source.
        /// </summary>
        /// <remarks>If the encoding is null, the default encoding will be used.</remarks>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Gets or sets the default overflow truncation policy to use
        /// when a value exceeds the maximum length of its column.
        /// </summary>
        public OverflowTruncationPolicy TruncationPolicy
        {
            get 
            { 
                return truncationPolicy; 
            }
            set
            {
                if (!Enum.IsDefined(typeof(OverflowTruncationPolicy), value))
                {
                    throw new ArgumentException(Resources.InvalidTruncationPolicy, "value");
                }
                truncationPolicy = value;
            }
        }

        /// <summary>
        /// Gets or sets the the number of rows containing skipped header information.
        /// </summary>
        public int HeaderRows { get; set; } = 0;

        /// <summary>
        /// Duplicates the options.
        /// </summary>
        /// <returns>The new options.</returns>
        public FixedLengthOptions Clone()
        {
            return (FixedLengthOptions)MemberwiseClone();
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
