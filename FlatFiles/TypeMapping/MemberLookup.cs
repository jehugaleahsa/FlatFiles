using System;
using System.Collections.Generic;
using System.Linq;

namespace FlatFiles.TypeMapping
{
    internal class MemberLookup
    {
        private readonly Dictionary<string, IMemberMapping> _lookup = new Dictionary<string, IMemberMapping>();
        private readonly Dictionary<Type, object> _factories = new Dictionary<Type, object>();
        private int _ignoredCount;

        public int WorkCount => _lookup.Count - _ignoredCount;

        public TMemberMapping GetOrAddMember<TMemberMapping>(IMemberAccessor member, Func<int, int, TMemberMapping> factory)
            where TMemberMapping : IMemberMapping
        {
            return GetOrAddMember(member.Name, factory);
        }

        public WriteOnlyPropertyMapping GetOrAddWriteOnlyMember(string name, Func<int, int, WriteOnlyPropertyMapping> factory)
        {
            string key = $"@WriteOnly_{name}";
            return GetOrAddMember(key, factory);
        }

        private TMapping GetOrAddMember<TMapping>(string key, Func<int, int, TMapping> factory)
            where TMapping : IMemberMapping
        {
            if (_lookup.TryGetValue(key, out var mapping))
            {
                return (TMapping)mapping;
            }

            int fileIndex = _lookup.Count;
            int workIndex = fileIndex - _ignoredCount;
            var newMapping = factory(fileIndex, workIndex);
            _lookup.Add(key, newMapping);
            return newMapping;
        }

        public IgnoredMapping AddIgnored()
        {
            var column = new IgnoredColumn();
            var mapping = new IgnoredMapping(column, _lookup.Count);
            string key = $"@Ignored_{mapping.FileIndex}";
            _lookup.Add(key, mapping);
            ++_ignoredCount;
            return mapping;
        }

        public IMemberMapping[] GetMappings()
        {
            return _lookup.Values.OrderBy(m => m.FileIndex).ToArray();
        }

        public Func<TEntity> GetFactory<TEntity>()
        {
            if (_factories.TryGetValue(typeof(TEntity), out var factory))
            {
                if (factory is Func<TEntity> entityFactory)
                {
                    return entityFactory;
                }

                if (factory is Func<object> objectFactory)
                {
                    return () => (TEntity)objectFactory();
                }
            }
            return null;
        }

        public void SetFactory<TEntity>(Func<TEntity> factory)
        {
            _factories.Add(typeof(TEntity), factory);
        }

        public void SetFactory(Type entityType, Func<object> factory)
        {
            _factories.Add(entityType, factory);
        }
    }
}
