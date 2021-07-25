using System;
using System.Reflection;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Provides the information needed to map a property to a column when writing.
    /// </summary>
    public interface IAutoMapResolver
    {
        /// <summary>
        /// Gets the index of the column in the file.
        /// </summary>
        /// <param name="member">The member to get the index for.</param>
        /// <returns>The index of the column in the file.</returns>
        int GetPosition(MemberInfo member);

        /// <summary>
        /// Gets the name of the column for a member.
        /// </summary>
        /// <param name="member">The member to generate the column name for.</param>
        /// <returns>The generated name.</returns>
        string GetColumnName(MemberInfo member);
    }
}
