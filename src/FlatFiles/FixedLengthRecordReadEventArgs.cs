using System;

namespace FlatFiles
{
    /// <summary>
    /// Holds the information related to an unpartitioned, unparsed fixed length record.
    /// </summary>
    public class FixedLengthRecordReadEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of a FixedLengthRecordReadEventArgs.
        /// </summary>
        internal FixedLengthRecordReadEventArgs(string record)
        {
            this.Record = record;
        }

        /// <summary>
        /// Gets the unpartitioned, unparsed record values read from the source file.
        /// </summary>
        public string Record { get; private set; }

        /// <summary>
        /// Gets or sets whether the record should be skipped.
        /// </summary>
        public bool IsSkipped { get; set; }
    }
}
