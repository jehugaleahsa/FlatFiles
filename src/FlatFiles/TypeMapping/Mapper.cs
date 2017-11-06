using System;
using System.Linq;

namespace FlatFiles.TypeMapping
{
    internal interface IMapper
    {
        IMemberAccessor Member { get; }

        Func<object[], object> GetReader();

        Action<object, object[]> GetWriter();
    }

    internal interface IMapper<TEntity>: IMapper
    {
        int WorkCount { get; }

        new Func<object[], TEntity> GetReader();

        new Action<TEntity, object[]> GetWriter();
    }

    internal class Mapper<TEntity> : IMapper<TEntity>
    {
        private readonly MemberLookup lookup;
        private readonly ICodeGenerator codeGenerator;
        private readonly IMemberAccessor accessor;

        public Mapper(MemberLookup lookup, ICodeGenerator codeGenerator)
            : this(lookup, codeGenerator, null)
        {
        }

        public Mapper(MemberLookup lookup, ICodeGenerator codeGenerator, IMemberAccessor accessor)
        {
            this.lookup = lookup;
            this.codeGenerator = codeGenerator;
            this.accessor = accessor;
        }

        public IMemberAccessor Member
        {
            get { return accessor; }
        }

        public int WorkCount
        {
            get { return lookup.WorkCount; }
        }

        public Func<object[], TEntity> GetReader()
        {
            var factory = lookup.GetFactory<TEntity>();
            if (factory == null)
            {
                factory = codeGenerator.GetFactory<TEntity>();
            }
            var mappings = lookup.GetMappings();
            var entityMembers = getEntityMembers(mappings);
            var entityMappers = entityMembers.Select(m => getMapper(m)).ToArray();
            var memberMappings = getMemberMappings(mappings);
            var setter = codeGenerator.GetReader<TEntity>(memberMappings);
            return (values) =>
            {
                var entity = factory();
                foreach (var entityMapper in entityMappers)
                {
                    var reader = entityMapper.GetReader();
                    var result = reader(values);
                    entityMapper.Member.SetValue(entity, result);
                }
                setter(entity, values);
                return entity;
            };
        }

        private IMapper getMapper(IMemberAccessor member)
        {
            var entityType = member.Type;
            var mapperType = typeof(Mapper<>).MakeGenericType(entityType);
            var mapper = (IMapper)Activator.CreateInstance(mapperType, lookup, codeGenerator, member);
            return mapper;

        }

        Func<object[], object> IMapper.GetReader()
        {
            return (values) => GetReader()(values);
        }

        public Action<TEntity, object[]> GetWriter()
        {
            var mappings = lookup.GetMappings();
            var memberMappings = getMemberMappings(mappings);
            var entityMembers = getEntityMembers(mappings);
            var entityMappers = entityMembers.Select(m => getMapper(m)).ToArray();
            var getter = codeGenerator.GetWriter<TEntity>(memberMappings);
            return (entity, values) =>
            {
                getter(entity, values);
                foreach (var entityMapper in entityMappers)
                {
                    var writer = entityMapper.GetWriter();
                    var nested = entityMapper.Member.GetValue(entity);
                    writer(nested, values);
                }
            };
        }

        Action<object, object[]> IMapper.GetWriter()
        {
            return (entity, values) => GetWriter()((TEntity)entity, values);
        }

        private IMemberMapping[] getMemberMappings(IMemberMapping[] mappings)
        {
            var memberMappings = mappings
                .Where(m => m.Member != null)
                .Where(m => accessor?.Name == m.Member.ParentAccessor?.Name)
                .ToArray();
            return memberMappings;
        }

        private IMemberAccessor[] getEntityMembers(IMemberMapping[] mappings)
        {
            var entityMappings = mappings
                .Where(m => m.Member != null)
                .Where(m => accessor?.Name != m.Member.ParentAccessor?.Name)
                .Where(m => m.Member.Name.StartsWith(accessor?.Name ?? String.Empty))
                .Select(m => getParentAccessor(m))
                .GroupBy(p => p.Name)
                .Select(g => g.First())
                .ToArray();
            return entityMappings;
        }

        private IMemberAccessor getParentAccessor(IMemberMapping mapping)
        {
            string accessorName = accessor?.Name ?? String.Empty;
            var childAccessor = mapping.Member;
            var parentAccessor = childAccessor.ParentAccessor;
            while (parentAccessor != null && accessorName != parentAccessor.Name)
            {
                childAccessor = parentAccessor;
                parentAccessor = childAccessor.ParentAccessor;
            }
            return childAccessor;
        }
    }
}
