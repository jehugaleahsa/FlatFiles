using System;
using System.Text;

namespace FlatFiles
{
    /// <summary>
    /// Holds configuration options for the SeparatedValueParser.
    /// </summary>
    public sealed class SeparatedValueOptions : ICloneable
    {
        /// <summary>
        /// Initializes a new instance of a SeparatedValueParserOptions.
        /// </summary>
        public SeparatedValueOptions()
        {
            Separator = ",";
            RecordSeparator = Environment.NewLine;
        }

        /// <summary>
        /// Gets or sets the separator used to separate the columns.
        /// </summary>
        public string Separator { get; set; }

        /// <summary>
        /// Gets or sets the separator used to separate the records.
        /// </summary>
        public string RecordSeparator { get; set; }

        /// <summary>
        /// Gets or sets whether the first record is the schema.
        /// </summary>
        public bool IsFirstRecordSchema { get; set; }

        /// <summary>
        /// Gets or sets the encoding of the text source.
        /// </summary>
        /// <remarks>If the encoding is null, the default encoding will be used.</remarks>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Duplicates the options.
        /// </summary>
        /// <returns>The new options.</returns>
        public SeparatedValueOptions Clone()
        {
            return (SeparatedValueOptions)MemberwiseClone();
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
