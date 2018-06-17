using System;

namespace FlatFiles
{
    /// <summary>
    /// Holds the information related to a partitioned, unparsed fixed length record.
    /// </summary>
    public class FixedLengthRecordPartitionedEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of a FixedLengthRecordPartitionedEventArgs.
        /// </summary>
        internal FixedLengthRecordPartitionedEventArgs(string[] values)
        {
            Values = values;
        }

        /// <summary>
        /// Gets the partitioned, unparsed record values read from the source file.
        /// </summary>
        public string[] Values { get; private set; }

        /// <summary>
        /// Gets or sets whether the record should be skipped.
        /// </summary>
        public bool IsSkipped { get; set; }
    }
}
