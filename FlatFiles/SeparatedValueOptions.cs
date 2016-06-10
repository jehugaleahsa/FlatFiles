using System;
using System.Text;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Holds configuration options for the SeparatedValueParser.
    /// </summary>
    public sealed class SeparatedValueOptions
    {
        private string separator;
        private string recordSeparator;

        /// <summary>
        /// Initializes a new instance of a SeparatedValueParserOptions.
        /// </summary>
        public SeparatedValueOptions()
        {
            Separator = ",";
            RecordSeparator = Environment.NewLine;
            Quote = '"';
        }

        /// <summary>
        /// Gets or sets the separator used to separate the columns.
        /// </summary>
        public string Separator
        {
            get
            {
                return separator;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException(Resources.EmptySeparator);
                }
                this.separator = value;
            }
        }

        /// <summary>
        /// Gets or sets the separator used to separate the records.
        /// </summary>
        public string RecordSeparator
        {
            get
            {
                return recordSeparator;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException(Resources.EmptyRecordSeparator);
                }
                this.recordSeparator = value;
            }
        }

        /// <summary>
        /// Gets or sets the character used to quote records containing special characters.
        /// </summary>
        public char Quote { get; set; }

        /// <summary>
        /// Gets or sets whether the first record is the schema.
        /// </summary>
        public bool IsFirstRecordSchema { get; set; }

        /// <summary>
        /// Gets or sets whether leading and trailing whitespace should be preserved when reading.
        /// </summary>
        public bool PreserveWhiteSpace { get; set; }

        /// <summary>
        /// Duplicates the options.
        /// </summary>
        /// <returns>The new options.</returns>
        public SeparatedValueOptions Clone()
        {
            return (SeparatedValueOptions)MemberwiseClone();
        }
    }
}
