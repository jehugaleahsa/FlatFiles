using System;
using System.Reflection;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Provides a column name for a member.
    /// </summary>
    public interface IAutoMapNameResolver
    {
        /// <summary>
        /// Gets the name of the column for a member.
        /// </summary>
        /// <param name="member">The member to generate the column name for.</param>
        /// <returns>The generated name.</returns>
        string ResolveName(MemberInfo member);
    }

    /// <summary>
    /// Provides a column name for a member.
    /// </summary>
    public sealed class AutoMapNameResolver : IAutoMapNameResolver
    {
        /// <summary>
        /// Gets the default auto-map name resolver.
        /// </summary>
        public static readonly IAutoMapNameResolver Default = new DefaultAutoMapNameResolver();
        private readonly Func<MemberInfo, string> resolver;

        private AutoMapNameResolver(Func<MemberInfo, string> resolver)
        {
            this.resolver = resolver;
        }

        /// <summary>
        /// Gets a name resolver for the given delegate.
        /// </summary>
        /// <param name="resolver">A delegate for naming columns.</param>
        /// <returns>The generated resolver.</returns>
        public static IAutoMapNameResolver For(Func<MemberInfo, string> resolver)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }
            return new AutoMapNameResolver(resolver);
        }

        /// <summary>
        /// Gets the name of the column for a member.
        /// </summary>
        /// <param name="member">The member to generate the column name for.</param>
        /// <returns>The generated name.</returns>
        public string ResolveName(MemberInfo member)
        {
            return resolver(member);
        }
    }

    internal class DefaultAutoMapNameResolver : IAutoMapNameResolver
    {
        public string ResolveName(MemberInfo member)
        {
            return member.Name;
        }
    }
}
