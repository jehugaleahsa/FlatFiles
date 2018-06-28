using System;
using System.Linq;
using System.Reflection;
using FlatFiles.Properties;

namespace FlatFiles.TypeMapping
{
    internal interface IMapper
    {
        IMemberAccessor Member { get; }

        int WorkCount { get; }

        Func<IProcessMetadata, object[], object> GetReader();

        Action<IProcessMetadata, object, object[]> GetWriter();
    }

    internal interface IMapper<TEntity>: IMapper
    {
        new Func<IProcessMetadata, object[], TEntity> GetReader();

        new Action<IProcessMetadata, TEntity, object[]> GetWriter();
    }

    internal class Mapper<TEntity> : IMapper<TEntity>
    {
        private readonly MemberLookup lookup;
        private readonly ICodeGenerator codeGenerator;
        private Func<IProcessMetadata, object[], TEntity> cachedReader;
        private Action<IProcessMetadata, TEntity, object[]> cachedWriter;

        public Mapper(MemberLookup lookup, ICodeGenerator codeGenerator)
            : this(lookup, codeGenerator, null)
        {
        }

        public Mapper(MemberLookup lookup, ICodeGenerator codeGenerator, IMemberAccessor member)
        {
            this.lookup = lookup;
            this.codeGenerator = codeGenerator;
            Member = member;
        }

        public IMemberAccessor Member { get; }

        public int WorkCount => lookup.WorkCount;

        public Func<IProcessMetadata, object[], TEntity> GetReader()
        {
            if (cachedReader != null)
            {
                return cachedReader;
            }
            var factory = lookup.GetFactory<TEntity>() ?? codeGenerator.GetFactory<TEntity>();
            var mappings = lookup.GetMappings();
            var memberMappings = GetMemberMappings(mappings);
            var setter = codeGenerator.GetReader<TEntity>(memberMappings);
            var nullChecker = GetNullChecker(memberMappings);
            var nestedMappers = GetNestedMappers(mappings);
            if (nestedMappers.Any())
            {
                cachedReader = (metadata, values) =>
                {
                    var entity = factory();
                    nullChecker(values);
                    setter(metadata, entity, values);
                    foreach (var nestedMapper in nestedMappers)
                    {
                        var nestedReader = nestedMapper.GetReader();
                        var result = nestedReader(metadata, values);
                        nestedMapper.Member.SetValue(entity, result);
                    }
                    return entity;
                };
            }
            else
            {
                cachedReader = (metadata, values) =>
                {
                    var entity = factory();
                    nullChecker(values);
                    setter(metadata, entity, values);
                    return entity;
                };
            }
            return cachedReader;
        }

        private Action<object[]> GetNullChecker(IMemberMapping[] memberMappings)
        {
            var nonNullLookup = memberMappings
                .Where(m => !m.Member.Type.GetTypeInfo().IsClass)
                .Where(m => Nullable.GetUnderlyingType(m.Member.Type) == null)
                .ToDictionary(m => m.WorkIndex);
            if (nonNullLookup.Count == 0)
            {
                return values => { };
            }
            return values =>
            {
                for (int index = 0; index != values.Length; ++index)
                {
                    if (values[index] == null && nonNullLookup.TryGetValue(index, out var mapping))
                    {
                        string message = String.Format(null, Resources.AssignNullToStruct, mapping.Member.Name);
                        throw new FlatFileException(message);
                    }
                }
            };
        }

        Func<IProcessMetadata, object[], object> IMapper.GetReader()
        {
            var reader = GetReader();
            return (metadata, values) => reader(metadata, values);
        }

        public Action<IProcessMetadata, TEntity, object[]> GetWriter()
        {
            if (cachedWriter != null)
            {
                return cachedWriter;
            }
            var mappings = lookup.GetMappings();
            var memberMappings = GetMemberMappings(mappings);
            var getter = codeGenerator.GetWriter<TEntity>(memberMappings);
            var nestedMappers = GetNestedMappers(mappings);
            if (nestedMappers.Any())
            {
                cachedWriter = (metadata, entity, values) =>
                {
                    getter(metadata, entity, values);
                    foreach (var nestedMapper in nestedMappers)
                    {
                        var nested = nestedMapper.Member.GetValue(entity);
                        var writer = nestedMapper.GetWriter();
                        writer(metadata, nested, values);
                    }
                };
            }
            else
            {
                cachedWriter = getter;
            }
            return cachedWriter;
        }

        Action<IProcessMetadata, object, object[]> IMapper.GetWriter()
        {
            var writer = GetWriter();
            return (metadata, entity, values) => writer(metadata, (TEntity)entity, values);
        }

        private IMemberMapping[] GetMemberMappings(IMemberMapping[] mappings)
        {
            var memberMappings = mappings
                .Where(m => m.Member != null)
                .Where(m => Member?.Name == m.Member.ParentAccessor?.Name)
                .ToArray();
            return memberMappings;
        }

        private IMapper[] GetNestedMappers(IMemberMapping[] mappings)
        {
            var mappers = mappings
                .Where(m => m.Member != null)
                .Where(m => Member?.Name != m.Member.ParentAccessor?.Name)
                .Where(m => m.Member.Name.StartsWith(Member?.Name ?? String.Empty))
                .Select(GetParentAccessor)
                .GroupBy(p => p.Name)
                .Select(g => g.First())
                .Select(GetMapper)
                .ToArray();
            return mappers;
        }

        private IMemberAccessor GetParentAccessor(IMemberMapping mapping)
        {
            string accessorName = Member?.Name ?? String.Empty;
            var childAccessor = mapping.Member;
            var parentAccessor = childAccessor.ParentAccessor;
            while (parentAccessor != null && accessorName != parentAccessor.Name)
            {
                childAccessor = parentAccessor;
                parentAccessor = childAccessor.ParentAccessor;
            }
            return childAccessor;
        }

        private IMapper GetMapper(IMemberAccessor member)
        {
            var entityType = member.Type;
            var mapperType = typeof(Mapper<>).MakeGenericType(entityType);
            var mapper = (IMapper)Activator.CreateInstance(mapperType, lookup, codeGenerator, member);
            return mapper;
        }
    }
}
