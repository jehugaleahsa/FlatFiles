using System;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Holds information about the currently running process.
    /// </summary>
    public interface IProcessContext
    {
        /// <summary>
        /// Gets the schema being used to process the records.
        /// </summary>
        ISchema Schema { get; }

        /// <summary>
        /// Gets the global options being used to process the records.
        /// </summary>
        IOptions Options { get; }
    }

    internal class ProcessContext : IProcessContext
    {
        public ISchema Schema { get; set; }

        public IOptions Options { get; set; }
    }
}
