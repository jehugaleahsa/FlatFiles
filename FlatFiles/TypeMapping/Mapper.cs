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

        Func<object[], object> GetReader();

        Action<object, object[]> GetWriter();
    }

    internal interface IMapper<TEntity>: IMapper
    {
        new Func<object[], TEntity> GetReader();

        new Action<TEntity, object[]> GetWriter();
    }

    internal class Mapper<TEntity> : IMapper<TEntity>
    {
        private readonly MemberLookup lookup;
        private readonly ICodeGenerator codeGenerator;
        private Func<object[], TEntity> cachedReader;
        private Action<TEntity, object[]> cachedWriter;

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

        public Func<object[], TEntity> GetReader()
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
                cachedReader = values =>
                {
                    var entity = factory();
                    nullChecker(values);
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
                cachedReader = values =>
                {
                    var entity = factory();
                    nullChecker(values);
                    setter(entity, values);
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

        Func<object[], object> IMapper.GetReader()
        {
            var reader = GetReader();
            return values => reader(values);
        }

        public Action<TEntity, object[]> GetWriter()
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
