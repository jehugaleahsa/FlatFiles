using System;
using FlatFiles.Resources;

namespace FlatFiles
{
    /// <summary>
    /// Holds configuration settings for the FixedLengthParser class.
    /// </summary>
    public sealed class FixedLengthOptions
    {
        private FixedAlignment alignment;
        private OverflowTruncationPolicy truncationPolicy;

        /// <summary>
        /// Initializes a new instance of a FixedLengthParserOptions.
        /// </summary>
        public FixedLengthOptions()
        {
            FillCharacter = ' ';
            RecordSeparator = Environment.NewLine;
            alignment = FixedAlignment.LeftAligned;
            truncationPolicy = OverflowTruncationPolicy.TruncateLeading;
        }

        /// <summary>
        /// Gets or sets the character used to buffer values in a column.
        /// </summary>
        /// <remarks>The fill character can be controlled at the column level using the Window class.</remarks>
        public char FillCharacter { get; set; }

        /// <summary>
        /// Gets or sets the string that indicates the end of a record.
        /// </summary>
        public string RecordSeparator { get; set; }

        /// <summary>
        /// Gets or sets whether the first record in the source holds header information and should be skipped.
        /// </summary>
        public bool IsFirstRecordHeader { get; set; }

        /// <summary>
        /// Gets or sets a filter to use to skip records prior to record being partitioned.
        /// </summary>
        public Func<string, bool> UnpartitionedRecordFilter { get; set; }

        /// <summary>
        /// Gets or sets a filter to use to skip records after the record is partitioned.
        /// </summary>
        public Func<string[], bool> PartitionedRecordFilter { get; set; }

        /// <summary>
        /// Gets or sets the default alignment for the values in the fixed length file.
        /// </summary>
        /// <remarks>The alignment can be controlled at the columnm level using the Window class.</remarks>
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
                    throw new ArgumentException(SharedResources.InvalidAlignment, nameof(value));
                }
                alignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the default overflow truncation policy to use when a value exceeds the maximum length of its column.
        /// </summary>
        /// <remarks>The trunaction policy can be controlled at the column level using the Window class.</remarks>
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
                    throw new ArgumentException(SharedResources.InvalidTruncationPolicy, "value");
                }
                truncationPolicy = value;
            }
        }

        /// <summary>
        /// Raised when an error occurs while processing a record.
        /// </summary>
        public EventHandler<ProcessingErrorEventArgs> ErrorHandler;

        /// <summary>
        /// Duplicates the options.
        /// </summary>
        /// <returns>The new options.</returns>
        public FixedLengthOptions Clone()
        {
            return (FixedLengthOptions)MemberwiseClone();
        }
    }
}
