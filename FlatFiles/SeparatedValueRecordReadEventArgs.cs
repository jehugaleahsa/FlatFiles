using System;

namespace FlatFiles
{
    /// <summary>
    /// Holds the information related to an unparsed separated value record.
    /// </summary>
    public class SeparatedValueRecordReadEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of a SeparatedValueRecordReadEventArgs.
        /// </summary>
        internal SeparatedValueRecordReadEventArgs(string[] values)
        {
            Values = values;
        }

        /// <summary>
        /// Gets the unparsed record values read from the source file.
        /// </summary>
        public string[] Values { get; private set; }

        /// <summary>
        /// Gets or sets whether the record should be skipped.
        /// </summary>
        public bool IsSkipped { get; set; }
    }
}
