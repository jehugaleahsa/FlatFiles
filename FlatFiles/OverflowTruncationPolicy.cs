using System;

namespace FlatFiles
{
    /// <summary>
    /// Specifies how to truncate columns when the data exceeds to maximum width.
    /// </summary>
    public enum OverflowTruncationPolicy
    {
        /// <summary>
        /// Keep the end of the data by removing the leading text.
        /// </summary>
        TruncateLeading,
        /// <summary>
        /// Keep the front of the data by removing the trailing text.
        /// </summary>
        TruncateTrailing
    }
}
