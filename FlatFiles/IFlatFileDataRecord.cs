#if NET462 || NETSTANDARD2_0 || NETCOREAPP

using System;
using System.Data;

namespace FlatFiles
{
    /// <summary>
    /// Provides access to the column values within each row for a <see cref="FlatFileDataReader"/>.
    /// </summary>
    public interface IFlatFileDataRecord : IDataRecord
    {
        /// <summary>
        /// Gets the DateTime value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The DateTime value at the given index.</returns>
        DateTimeOffset GetDateTimeOffset(int i);

        /// <summary>
        /// Gets the sbyte value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The sbyte value at the given index.</returns>
        sbyte GetSByte(int i);

        /// <summary>
        /// Gets the <see cref="TimeSpan"/> from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The string at the given index.</returns>
        TimeSpan GetTimeSpan(int i);

        /// <summary>
        /// Gets the unsigned short value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The unsigned short value at the given index.</returns>
        ushort GetUInt16(int i);

        /// <summary>
        /// Gets the unsigned int value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The unsigned int value at the given index.</returns>
        uint GetUInt32(int i);

        /// <summary>
        /// Gets the unsigned long value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The unsigned long value at the given index.</returns>
        ulong GetUInt64(int i);
    }
}

#endif