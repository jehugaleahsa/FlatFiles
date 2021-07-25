using System;
using System.Reflection;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Provides helper methods for creating auto-mapping matchers.
    /// </summary>
    public sealed class AutoMapMatcher : IAutoMapMatcher
    {
        /// <summary>
        /// Gets the default auto-mapping matcher (case-insensitive name comparison).
        /// </summary>
        public static readonly IAutoMapMatcher Default = new DefaultAutoMapMatcher();
        private readonly Func<IColumnDefinition, MemberInfo, bool> matcher;

        private AutoMapMatcher(Func<IColumnDefinition, MemberInfo, bool> matcher, bool useFallback)
        {
            this.matcher = matcher;
            this.UseFallback = useFallback;
        }

        /// <summary>
        /// Gets a matcher for the given delegate.
        /// </summary>
        /// <param name="matcher">A delegate that performs the auto-mapping matcher.</param>
        /// <param name="useFallback">Specifies whether to fallback on an exact name match if no matches are found.</param>
        /// <returns>The generated matcher.</returns>
        public static IAutoMapMatcher For(Func<IColumnDefinition, MemberInfo, bool> matcher, bool useFallback = true)
        {
            if (matcher == null)
            {
                throw new ArgumentNullException(nameof(matcher));
            }
            return new AutoMapMatcher(matcher, useFallback);
        }

        /// <summary>
        /// Gets a matcher for the given name resolver.
        /// </summary>
        /// <param name="resolver">The name resolver used to generate the column names when writing.</param>
        /// <param name="useFallback">Specifies whether to fallback on an exact name match if no matches are found.</param>
        /// <returns>The generated matcher.</returns>
        public static IAutoMapMatcher For(IAutoMapResolver resolver, bool useFallback = true)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }
            return new AutoMapMatcher((column, member) => column.ColumnName == resolver.GetColumnName(member), useFallback);
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

        private sealed class DefaultAutoMapMatcher : IAutoMapMatcher
        {
            public bool UseFallback => true;

            public bool IsMatch(IColumnDefinition columnDefinition, MemberInfo member)
            {
                var comparer = StringComparer.OrdinalIgnoreCase;
                return comparer.Equals(columnDefinition.ColumnName, member.Name);
            }
        }
    }
}
