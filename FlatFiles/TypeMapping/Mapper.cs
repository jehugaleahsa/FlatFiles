using System;
using System.Linq;

namespace FlatFiles.TypeMapping
{
    internal class Mapper<TEntity> : IMapper<TEntity>
    {
        private readonly MemberLookup lookup;
        private readonly ICodeGenerator codeGenerator;
        private Func<IRecordContext, object?[], TEntity>? cachedReader;
        private Action<IRecordContext, TEntity, object?[]>? cachedWriter;

        public Mapper(MemberLookup lookup, ICodeGenerator codeGenerator)
            : this(lookup, codeGenerator, null)
        {
        }

        public Mapper(MemberLookup lookup, ICodeGenerator codeGenerator, IMemberAccessor? member)
        {
            this.lookup = lookup;
            this.codeGenerator = codeGenerator;
            Member = member;
        }

        public IMemberAccessor? Member { get; }

        public int LogicalCount => lookup.LogicalCount;

        public Func<IRecordContext, object?[], TEntity> GetReader()
        {
            if (cachedReader != null)
            {
                return cachedReader;
            }
            var factory = lookup.GetFactory<TEntity>() ?? codeGenerator.GetFactory<TEntity>();
            var mappings = lookup.GetMappings();
            var memberMappings = GetReaderMemberMappings(mappings);
            var deserializer = codeGenerator.GetReader<TEntity>(memberMappings);
            var nestedMappers = GetNestedMappers(mappings);
            if (nestedMappers.Any())
            {
                cachedReader = (recordContext, values) =>
                {
                    var entity = factory();
                    deserializer(recordContext, entity, values);
                    foreach (var nestedMapper in nestedMappers)
                    {
                        var nestedReader = nestedMapper.GetReader();
                        var result = nestedReader(recordContext, values);
                        nestedMapper.Member!.SetValue(entity!, result);
                    }
                    return entity;
                };
            }
            else
            {
                cachedReader = (recordContext, values) =>
                {
                    var entity = factory();
                    deserializer(recordContext, entity, values);
                    return entity;
                };
            }
            return cachedReader;
        }

        Func<IRecordContext, object?[], object?> IMapper.GetReader()
        {
            var reader = GetReader();
            return (metadata, values) => reader(metadata, values);
        }

        public Action<IRecordContext, TEntity, object?[]> GetWriter()
        {
            if (cachedWriter != null)
            {
                return cachedWriter;
            }
            var mappings = lookup.GetMappings();
            var memberMappings = GetWriterMemberMappings(mappings);
            var serializer = codeGenerator.GetWriter<TEntity>(memberMappings);
            var nestedMappers = GetNestedMappers(mappings);
            if (nestedMappers.Any())
            {
                cachedWriter = (metadata, entity, values) =>
                {
                    serializer(metadata, entity, values);
                    foreach (var nestedMapper in nestedMappers)
                    {
                        var nested = nestedMapper.Member!.GetValue(entity!);
                        var writer = nestedMapper.GetWriter();
                        writer(metadata, nested, values);
                    }
                };
            }
            else
            {
                cachedWriter = serializer;
            }
            return cachedWriter;
        }

        Action<IRecordContext, object?, object?[]> IMapper.GetWriter()
        {
            var writer = GetWriter();
            return (metadata, entity, values) => writer(metadata, (TEntity)entity!, values);
        }

        private IMemberMapping[] GetReaderMemberMappings(IMemberMapping[] mappings)
        {
            var memberMappings = mappings
                .Where(m => m.Member != null || m.Reader != null)
                .Where(m => Member?.Name == m.Member?.ParentAccessor?.Name)
                .ToArray();
            return memberMappings;
        }

        private IMemberMapping[] GetWriterMemberMappings(IMemberMapping[] mappings)
        {
            var memberMappings = mappings
                .Where(m => m.Member != null || m.Writer != null)
                .Where(m => Member?.Name == m.Member?.ParentAccessor?.Name)
                .ToArray();
            return memberMappings;
        }

        private IMapper[] GetNestedMappers(IMemberMapping[] mappings)
        {
            var mappers = mappings
                .Where(m => m.Member != null)
                .Where(m => Member?.Name != m.Member!.ParentAccessor?.Name)
                .Where(m => m.Member!.Name.StartsWith(Member?.Name ?? string.Empty))
                .Select(GetParentAccessor)
                .GroupBy(p => p.Name)
                .Select(g => g.First())
                .Select(GetMapper)
                .ToArray();
            return mappers;
        }

        private IMemberAccessor GetParentAccessor(IMemberMapping mapping)
        {
            string accessorName = Member?.Name ?? string.Empty;
            var childAccessor = mapping.Member!;
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
            var mapper = (IMapper)Activator.CreateInstance(mapperType, lookup, codeGenerator, member)!;
            return mapper;
        }
    }
}
