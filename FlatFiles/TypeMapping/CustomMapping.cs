using System;
using System.Linq.Expressions;
using System.Reflection;
using FlatFiles.Properties;

namespace FlatFiles.TypeMapping
{
    internal sealed class CustomMapping<TEntity> : ICustomMapping<TEntity>, ICustomMapping, IMemberMapping
    {
        public CustomMapping(IColumnDefinition column, int physicalIndex, int logicalIndex)
        {
            ColumnDefinition = column;
            PhysicalIndex = physicalIndex;
            LogicalIndex = logicalIndex;
        }

        public IMemberAccessor? Member => null;

        public Action<IColumnContext?, object?, object?>? Reader { get; private set; }

        public Action<IColumnContext?, object?, object?[]>? Writer { get; private set; }

        public IColumnDefinition ColumnDefinition { get; }

        public int PhysicalIndex { get; }

        public int LogicalIndex { get; }

        public ICustomMapping<TEntity> WithReader<TProp>(Expression<Func<TEntity, TProp>>? reader)
        {
            var newReader = GetReader(reader);
            return WithReader(newReader);
        }

        private static Action<IColumnContext?, TEntity, object?>? GetReader<TProp>(Expression<Func<TEntity, TProp>>? reader)
        {
            if (reader == null)
            {
                return null;
            }
            var context = Expression.Parameter(typeof(IColumnContext), "context");
            var entity = Expression.Parameter(typeof(TEntity), "entity");
            var value = Expression.Parameter(typeof(object), "value");
            var member = GetMemberExpression(entity, reader.Body);
            var readerBuilder = Expression.Lambda<Action<IColumnContext?, TEntity, object?>>(
                Expression.Assign(member, Expression.Convert(value, typeof(TProp))),
                context,
                entity,
                value
            );
            return readerBuilder.Compile();
        }

        private static Expression GetMemberExpression(Expression entityParameter, Expression expression)
        {
            if (expression == null || expression is not MemberExpression memberExpression)
            {
                throw new ArgumentException(Resources.BadPropertySelector, nameof(expression));
            }
            var memberInfo = memberExpression.Member;
            if (memberInfo is PropertyInfo propertyInfo)
            {
                if (memberInfo.DeclaringType!.GetTypeInfo().IsAssignableFrom(typeof(TEntity)))
                {
                    return Expression.Property(entityParameter, propertyInfo);
                }
                var nestedMember = GetMemberExpression(entityParameter, memberExpression.Expression);
                return Expression.Property(nestedMember, propertyInfo);
            }
            if (memberInfo is FieldInfo fieldInfo)
            {
                if (memberInfo.DeclaringType!.GetTypeInfo().IsAssignableFrom(typeof(TEntity)))
                {
                    return Expression.Field(entityParameter, fieldInfo);
                }
                var nestedMember = GetMemberExpression(entityParameter, memberExpression.Expression);
                return Expression.Field(nestedMember, fieldInfo);
            }
            throw new FlatFileException(Resources.BadPropertySelector);
        }

        public ICustomMapping<TEntity> WithReader(Action<TEntity, object?>? reader)
        {
            Reader = reader == null ? null : (ctx, e, v) => reader((TEntity)e!, v);
            return this;
        }

        public ICustomMapping<TEntity> WithReader(Action<IColumnContext?, TEntity, object?>? reader)
        {
            Reader = reader == null ? null : (ctx, e, v) => reader(ctx, (TEntity)e!, v);
            return this;
        }

        ICustomMapping ICustomMapping.WithReader(Action<object?, object?>? reader)
        {
            Reader = reader == null ? null : (ctx, e, v) => reader(e, v);
            return this;
        }

        ICustomMapping ICustomMapping.WithReader(Action<IColumnContext?, object?, object?>? reader)
        {
            Reader = reader;
            return this;
        }

        public ICustomMapping<TEntity> WithWriter(Action<TEntity, object?[]>? writer)
        {
            Writer = writer == null ? null : (ctx, e, v) => writer((TEntity)e!, v);
            return this;
        }

        public ICustomMapping<TEntity> WithWriter(Action<IColumnContext?, TEntity, object?[]>? writer)
        {
            Writer = writer == null ? null : (ctx, e, v) => writer(ctx, (TEntity)e!, v);
            return this;
        }

        ICustomMapping ICustomMapping.WithWriter(Action<object?, object?[]>? writer)
        {
            Writer = writer == null ? null : (ctx, e, v) => writer(e, v);
            return this;
        }

        ICustomMapping ICustomMapping.WithWriter(Action<IColumnContext?, object?, object?[]>? writer)
        {
            Writer = writer;
            return this;
        }

        public ICustomMapping<TEntity> WithWriter<TProp>(Func<TEntity, TProp>? writer)
        {
            Writer = writer == null ? null : (ctx, e, v) =>
            {
                v[LogicalIndex] = writer((TEntity)e!);
            };
            return this;
        }

        public ICustomMapping<TEntity> WithWriter<TProp>(Func<IColumnContext?, TEntity, TProp>? writer)
        {
            Writer = writer == null ? null : (ctx, e, v) =>
            {
                v[LogicalIndex] = writer(ctx, (TEntity)e!);
            };
            return this;
        }

        ICustomMapping ICustomMapping.WithWriter(Func<object?, object?>? writer)
        {
            Writer = writer == null ? null : (ctx, e, v) =>
            {
                v[LogicalIndex] = writer(e);
            };
            return this;
        }

        ICustomMapping ICustomMapping.WithWriter(Func<IColumnContext?, object?, object?>? writer)
        {
            Writer = writer == null ? null : (ctx, e, v) =>
            {
                v[LogicalIndex] = writer(ctx, e);
            };
            return this;
        }
    }
}
