using System;

namespace FlatFiles
{
    /// <summary>
    /// Metadata associated with the file processing.
    /// </summary>
    public interface IProcessMetadata
    {
        /// <summary>
        /// Gets the schema being used by the process.
        /// </summary>
        ISchema Schema { get; }

        /// <summary>
        /// Gets the shared options used to configure the reader/writer.
        /// </summary>
        IOptions Options { get; }

        /// <summary>
        /// Gets the number of records parsed so far, including skipped records.
        /// </summary>
        int RecordCount { get; }

        /// <summary>
        /// Gets the number of records parsed so far, excluding skipped records.
        /// </summary>
        int LogicalRecordCount { get; }
    }

    internal class FixedLengthProcessMetadata : IProcessMetadata
    {
        public FixedLengthSchema Schema { get; set; }

        ISchema IProcessMetadata.Schema => Schema;

        public FixedLengthOptions Options { get; set; }

        IOptions IProcessMetadata.Options => Options;

        public int RecordCount { get; set; }

        public int LogicalRecordCount { get; set; }
    }

    internal class SeparatedValueProcessMetadata : IProcessMetadata
    {
        public SeparatedValueSchema Schema { get; set; }

        ISchema IProcessMetadata.Schema => Schema;

        public SeparatedValueOptions Options { get; set; }

        IOptions IProcessMetadata.Options => Options;

        public int RecordCount { get; set; }

        public int LogicalRecordCount { get; set; }
    }
}
