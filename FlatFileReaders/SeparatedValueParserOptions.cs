using System;
using System.Text;

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
        public string Separator { get; set; }

        /// <summary>
        /// Gets or sets whether the first record is the schema.
        /// </summary>
        public bool IsFirstRecordSchema { get; set; }

        /// <summary>
        /// Gets or sets the encoding of the text source.
        /// </summary>
        /// <remarks>If the encoding is null, the default encoding will be used.</remarks>
        public Encoding Encoding { get; set; }
    }
}
