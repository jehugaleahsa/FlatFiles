using System;

namespace FlatFiles
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

    /// <summary>
    /// Holds information about the currently running process.
    /// </summary>
    public interface IFixedLengthProcessContext : IProcessContext
    {
        /// <summary>
        /// Gets the schema being used to process the records.
        /// </summary>
        new FixedLengthSchema Schema { get; }

        /// <summary>
        /// Gets the global options being used to process the records.
        /// </summary>
        new FixedLengthOptions Options { get; }
    }

    /// <summary>
    /// Holds information about the currently running process.
    /// </summary>
    public interface ISeparatedValueProcessContext : IProcessContext
    {
        /// <summary>
        /// Gets the schema being used to process the records.
        /// </summary>
        new SeparatedValueSchema Schema { get; }

        /// <summary>
        /// Gets the global options being used to process the records.
        /// </summary>
        new SeparatedValueOptions Options { get; }
    }

    internal class FixedLengthProcessContext : IFixedLengthProcessContext
    {
        public FixedLengthSchema Schema { get; set; }

        public FixedLengthOptions Options { get; set; }

        ISchema IProcessContext.Schema => Schema;

        IOptions IProcessContext.Options => Options;
    }

    internal class SeparatedValueProcessContext : ISeparatedValueProcessContext
    {
        public SeparatedValueSchema Schema { get; set; }

        public SeparatedValueOptions Options { get; set; }

        ISchema IProcessContext.Schema => Schema;

        IOptions IProcessContext.Options => Options;
    }

    internal class ProcessContext : IProcessContext
    {
        public ISchema Schema { get; set; }

        public IOptions Options { get; set; }
    }
}
