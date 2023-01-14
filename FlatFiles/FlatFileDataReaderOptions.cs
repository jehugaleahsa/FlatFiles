#if NET462 || NETSTANDARD2_0 || NETCOREAPP

using System;

namespace FlatFiles
{
    /// <summary>
    /// Holds configuration settings for the FlatFileDataReader class.
    /// </summary>
    public sealed class FlatFileDataReaderOptions
    {
        /// <summary>
        /// Initializes a new instance of FlatFileDataReaderOptions.
        /// </summary>
        public FlatFileDataReaderOptions()
        {
        }

        /// <summary>
        /// Gets or sets whether <see cref="DBNull"/> should be returned instead of null.
        /// </summary>
        public bool IsDBNullReturned { get; set; } = true;

        /// <summary>
        /// Gets or sets whether an <see cref="InvalidCastException" /> should be thrown if 
        /// <see cref="FlatFileDataReader.GetString(int)" /> is called when the value is null.
        /// </summary>
        public bool IsNullStringAllowed { get; set; }
    }
}

#endif