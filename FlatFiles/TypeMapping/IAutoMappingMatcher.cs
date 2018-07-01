using System;
using System.Reflection;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Determines if a column should be mapped to a property or field.
    /// </summary>
    public interface IAutoMappingMatcher
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

    /// <summary>
    /// Provides helper methods for creating auto-mapping matchers.
    /// </summary>
    public sealed class AutoMappingMatcher : IAutoMappingMatcher
    {
        /// <summary>
        /// Gets the default auto-mapping matcher (case-insensitive name comparison).
        /// </summary>
        public static readonly IAutoMappingMatcher Default = new DefaultAutoMappingMatcher();
        private readonly Func<IColumnDefinition, MemberInfo, bool> matcher;

        private AutoMappingMatcher(Func<IColumnDefinition, MemberInfo, bool> matcher, bool useFallback)
        {
            this.matcher = matcher;
            this.UseFallback = useFallback;
        }

        /// <summary>
        /// Gets a matcher for the given delegate.
        /// </summary>
        /// <param name="matcher">A delegate that performs the auto-mapping matcher.</param>
        /// <param name="useFallback">Specifies whether to fallback on an exact name match if no other matcher is found.</param>
        /// <returns>An auto-mapping matcher.</returns>
        public static IAutoMappingMatcher For(Func<IColumnDefinition, MemberInfo, bool> matcher, bool useFallback = true)
        {
            if (matcher == null)
            {
                throw new ArgumentNullException(nameof(matcher));
            }
            return new AutoMappingMatcher(matcher, useFallback);
        }

        /// <summary>
        /// Gets or sets whether to fallback on an exact name match if no other matcher is found.
        /// </summary>
        public bool UseFallback { get; }

        /// <summary>
        /// Gets whether the column should be mapped to the property or field.
        /// </summary>
        /// <param name="columnDefinition">The column being mapped.</param>
        /// <param name="member">The property or field to inspect.</param>
        /// <returns>True if the column and property match; otherwise, false.</returns>
        public bool IsMatch(IColumnDefinition columnDefinition, MemberInfo member)
        {
            return matcher(columnDefinition, member);
        }
    }

    internal sealed class DefaultAutoMappingMatcher : IAutoMappingMatcher
    {
        public bool UseFallback => true;

        public bool IsMatch(IColumnDefinition columnDefinition, MemberInfo member)
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            return comparer.Equals(columnDefinition.ColumnName, member.Name);
        }
    }
}
