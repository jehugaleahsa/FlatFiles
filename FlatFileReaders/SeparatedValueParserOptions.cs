using System;

namespace FlatFileReaders
{
    /// <summary>
    /// Holds configuration options for the SeparatedValueParser.
    /// </summary>
    public sealed class SeparatedValueParserOptions
    {
        /// <summary>
        /// Initializes a new instance of a SeparatedValueParserOptions.
        /// </summary>
        public SeparatedValueParserOptions()
        {
            Separator = ",";
        }

        /// <summary>
        /// Gets or sets the separator used to separate record items.
        /// </summary>
        public string Separator
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether the first record is the schema.
        /// </summary>
        public bool IsFirstRecordSchema
        {
            get;
            set;
        }
    }
}
