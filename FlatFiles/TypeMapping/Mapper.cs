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
        private readonly MemberLookup _lookup;
        private readonly ICodeGenerator _codeGenerator;
        private Func<object[], TEntity> _cachedReader;
        private Action<TEntity, object[]> _cachedWriter;

        public Mapper(MemberLookup lookup, ICodeGenerator codeGenerator)
            : this(lookup, codeGenerator, null)
        {
        }

        public Mapper(MemberLookup lookup, ICodeGenerator codeGenerator, IMemberAccessor member)
        {
            _lookup = lookup;
            _codeGenerator = codeGenerator;
            Member = member;
        }

        public IMemberAccessor Member { get; }

        public int WorkCount => _lookup.WorkCount;

        public Func<object[], TEntity> GetReader()
        {
            if (_cachedReader != null)
            {
                return _cachedReader;
            }
            var factory = _lookup.GetFactory<TEntity>() ?? _codeGenerator.GetFactory<TEntity>();
            var mappings = _lookup.GetMappings();
            var memberMappings = GetMemberMappings(mappings);
            var setter = _codeGenerator.GetReader<TEntity>(memberMappings);
            var nullChecker = GetNullChecker(memberMappings);
            var nestedMappers = GetNestedMappers(mappings);
            if (nestedMappers.Any())
            {
                _cachedReader = values =>
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
                _cachedReader = values =>
                {
                    var entity = factory();
                    nullChecker(values);
                    setter(entity, values);
                    return entity;
                };
            }
            return _cachedReader;
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
                        string message = string.Format(null, Resources.AssignNullToStruct, mapping.Member.Name);
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
            if (_cachedWriter != null)
            {
                return _cachedWriter;
            }
            var mappings = _lookup.GetMappings();
            var memberMappings = GetMemberMappings(mappings);
            var getter = _codeGenerator.GetWriter<TEntity>(memberMappings);
            var nestedMappers = GetNestedMappers(mappings);
            if (nestedMappers.Any())
            {
                _cachedWriter = (entity, values) =>
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
                _cachedWriter = getter;
            }
            return _cachedWriter;
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
                .Where(m => m.Member.Name.StartsWith(Member?.Name ?? string.Empty))
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
            var mapper = (IMapper)Activator.CreateInstance(mapperType, _lookup, _codeGenerator, member);
            return mapper;
        }
    }
}
