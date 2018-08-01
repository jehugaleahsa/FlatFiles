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

    /// <summary>
    /// Provides the information needed to map a property to a column when writing.
    /// </summary>
    public sealed class AutoMapResolver : IAutoMapResolver
    {
        /// <summary>
        /// Gets the default auto-map resolver.
        /// </summary>
        public static readonly IAutoMapResolver Default = new DefaultAutoMapNameResolver();

        private readonly Func<MemberInfo, string> nameResolver;
        private readonly Func<MemberInfo, int> positionResolver;

        private AutoMapResolver(Func<MemberInfo, string> nameResolver, Func<MemberInfo, int> positionResolver)
        {
            this.nameResolver = nameResolver;
            this.positionResolver = positionResolver ?? ((m) => 0);
        }

        /// <summary>
        /// Gets a resolver for the given delegate(s).
        /// </summary>
        /// <param name="nameResolver">A delegate for getting the column names.</param>
        /// <param name="positionResolver">A delegate for getting the column positions.</param>
        /// <returns>The generated resolver.</returns>
        public static IAutoMapResolver For(Func<MemberInfo, string> nameResolver, Func<MemberInfo, int> positionResolver = null)
        {
            if (nameResolver == null)
            {
                throw new ArgumentNullException(nameof(nameResolver));
            }
            return new AutoMapResolver(nameResolver, positionResolver);
        }

        /// <summary>
        /// Gets the index of the column in the file.
        /// </summary>
        /// <param name="member">The member to get the index for.</param>
        /// <returns>The index of the column in the file.</returns>
        public int GetPosition(MemberInfo member)
        {
            return positionResolver(member);
        }

        /// <summary>
        /// Gets the name of the column for a member.
        /// </summary>
        /// <param name="member">The member to generate the column name for.</param>
        /// <returns>The generated name -or- null to indicate the member should not be mapped.</returns>
        public string GetColumnName(MemberInfo member)
        {
            return nameResolver(member);
        }
    }

    internal class DefaultAutoMapNameResolver : IAutoMapResolver
    {
        public int GetPosition(MemberInfo member)
        {
            return 0;
        }

        public string GetColumnName(MemberInfo member)
        {
            return member.Name;
        }
    }
}
