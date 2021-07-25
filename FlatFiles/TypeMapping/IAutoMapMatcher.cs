using System.Reflection;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Determines if a column should be mapped to a property or field.
    /// </summary>
    public interface IAutoMapMatcher
    {
        /// <summary>
        /// Gets or sets whether to fallback on an exact name match if no other matcher is found.
        /// </summary>
        bool UseFallback { get; }

        /// <summary>
        /// Gets whether the column should be mapped to the property or field.
        /// </summary>
        /// <param name="columnDefinition">The column being mapped.</param>
        /// <param name="member">The property or field to inspect.</param>
        /// <returns>True if the column and property match; otherwise, false.</returns>
        bool IsMatch(IColumnDefinition columnDefinition, MemberInfo member);
    }
}
