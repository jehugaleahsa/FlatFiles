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
        private Func<object[], TEntity> cachedReader;
        private Action<TEntity, object[]> cachedWriter;

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
            if (cachedReader != null)
            {
                return cachedReader;
            }
            var factory = lookup.GetFactory<TEntity>();
            if (factory == null)
            {
                factory = codeGenerator.GetFactory<TEntity>();
            }
            var mappings = lookup.GetMappings();
            var memberMappings = getMemberMappings(mappings);
            var setter = codeGenerator.GetReader<TEntity>(memberMappings);
            var nestedMappers = getNestedMappers(mappings);
            if (nestedMappers.Any())
            {
                cachedReader = (values) =>
                {
                    var entity = factory();
                    setter(entity, values);
                    foreach (var nestedMapper in nestedMappers)
                    {
                        var nestedReader = nestedMapper.GetReader();
                        var result = nestedReader(values);
                        nestedMapper.Member.SetValue(entity, result);
                    }
                    return entity;
                };
            }
            else
            {
                cachedReader = (values) =>
                {
                    var entity = factory();
                    setter(entity, values);
                    return entity;
                };
            }
            return cachedReader;
        }

        Func<object[], object> IMapper.GetReader()
        {
            var reader = GetReader();
            return (values) => reader(values);
        }

        public Action<TEntity, object[]> GetWriter()
        {
            if (cachedWriter != null)
            {
                return cachedWriter;
            }
            var mappings = lookup.GetMappings();
            var memberMappings = getMemberMappings(mappings);
            var getter = codeGenerator.GetWriter<TEntity>(memberMappings);
            var nestedMappers = getNestedMappers(mappings);
            if (nestedMappers.Any())
            {
                cachedWriter = (entity, values) =>
                {
                    getter(entity, values);
                    foreach (var nestedMapper in nestedMappers)
                    {
                        var nested = nestedMapper.Member.GetValue(entity);
                        var writer = nestedMapper.GetWriter();
                        writer(nested, values);
                    }
                };
            }
            else
            {
                cachedWriter = getter;
            }
            return cachedWriter;
        }

        Action<object, object[]> IMapper.GetWriter()
        {
            var writer = GetWriter();
            return (entity, values) => writer((TEntity)entity, values);
        }

        private IMemberMapping[] getMemberMappings(IMemberMapping[] mappings)
        {
            var memberMappings = mappings
                .Where(m => m.Member != null)
                .Where(m => accessor?.Name == m.Member.ParentAccessor?.Name)
                .ToArray();
            return memberMappings;
        }

        private IMapper[] getNestedMappers(IMemberMapping[] mappings)
        {
            var mappers = mappings
                .Where(m => m.Member != null)
                .Where(m => accessor?.Name != m.Member.ParentAccessor?.Name)
                .Where(m => m.Member.Name.StartsWith(accessor?.Name ?? String.Empty))
                .Select(m => getParentAccessor(m))
                .GroupBy(p => p.Name)
                .Select(g => g.First())
                .Select(m => getMapper(m))
                .ToArray();
            return mappers;
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

        private IMapper getMapper(IMemberAccessor member)
        {
            var entityType = member.Type;
            var mapperType = typeof(Mapper<>).MakeGenericType(entityType);
            var mapper = (IMapper)Activator.CreateInstance(mapperType, lookup, codeGenerator, member);
            return mapper;
        }
    }
}
