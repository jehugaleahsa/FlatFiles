using System;
using System.Collections.Generic;
using System.Linq;

namespace FlatFiles.TypeMapping
{
    internal class MemberLookup
    {
        private readonly Dictionary<string, IMemberMapping> lookup;
        private int ignoredCount;

        public MemberLookup()
        {
            this.lookup = new Dictionary<string, IMemberMapping>();
        }

        public TMemberMapping GetOrAddMember<TMemberMapping>(IMemberAccessor member, Func<int, int, TMemberMapping> factory)
            where TMemberMapping : IMemberMapping
        {
            if (lookup.TryGetValue(member.Name, out var mapping))
            {
                return (TMemberMapping)mapping;
            }
            else
            {
                int fileIndex = lookup.Count;
                int workIndex = lookup.Count - ignoredCount;
                var newMapping = factory(fileIndex, workIndex);
                lookup.Add(member.Name, newMapping);
                return newMapping;
            }
        }

        public IgnoredMapping AddIgnored()
        {
            var column = new IgnoredColumn();
            var mapping = new IgnoredMapping(column, lookup.Count);
            string key = $"@Ignored_{mapping.FileIndex}";
            lookup.Add(key, mapping);
            ++ignoredCount;
            return mapping;
        }

        public IMemberMapping[] GetMappings()
        {
            return lookup.Values.OrderBy(m => m.FileIndex).ToArray();
        }
    }
}
