using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class ColumnContextTester
    {
        [TestMethod]
        public void ShouldPassCorrectIndexesWhenReading()
        {
            const string data = "1,,Bob,,2018-06-30,,True";
            var mapper = SeparatedValueTypeMapper.Define(() => new Person());
            var colPhysicalIndexes = new List<int>();
            var colLogicalIndexes = new List<int>();
            var propPhysicalIndexes = new List<int>();
            var propLogicalIndexes = new List<int>();
            mapper.CustomMapping(new IndexTrackingColumn(new Int32Column("Id"), colPhysicalIndexes, colLogicalIndexes))
                .WithReader((ctx, x, v) =>
                {
                    propPhysicalIndexes.Add(ctx.PhysicalIndex);
                    propLogicalIndexes.Add(ctx.LogicalIndex);
                    x.Id = (int)v;
                });
            mapper.Ignored();
            mapper.CustomMapping(new IndexTrackingColumn(new StringColumn("Name"), colPhysicalIndexes, colLogicalIndexes))
                .WithReader((ctx, x, v) =>
                {
                    propPhysicalIndexes.Add(ctx.PhysicalIndex);
                    propLogicalIndexes.Add(ctx.LogicalIndex);
                    x.Name = (string)v;
                });
            mapper.Ignored();
            mapper.CustomMapping(new IndexTrackingColumn(new DateTimeColumn("CreatedOn") { InputFormat = "yyyy-MM-dd" }, colPhysicalIndexes, colLogicalIndexes))
                .WithReader((ctx, x, v) =>
                {
                    propPhysicalIndexes.Add(ctx.PhysicalIndex);
                    propLogicalIndexes.Add(ctx.LogicalIndex);
                    x.CreatedOn = (DateTime)v;
                });
            mapper.Ignored();
            mapper.CustomMapping(new IndexTrackingColumn(new BooleanColumn("IsActive"), colPhysicalIndexes, colLogicalIndexes))
                .WithReader((ctx, x, v) =>
                {
                    propPhysicalIndexes.Add(ctx.PhysicalIndex);
                    propLogicalIndexes.Add(ctx.LogicalIndex);
                    x.IsActive = (bool)v;
                });
            var reader = new StringReader(data);
            var typedReader = mapper.GetReader(reader);
            var results = typedReader.ReadAll().ToArray();

            var expectedPhysicalIndexes = new int[] { 0, 2, 4, 6 };
            CollectionAssert.AreEqual(expectedPhysicalIndexes, colPhysicalIndexes);
            CollectionAssert.AreEqual(expectedPhysicalIndexes, propPhysicalIndexes);

            var expectedLogicalIndexes = new int[] { 0, 1, 2, 3 };
            CollectionAssert.AreEqual(expectedLogicalIndexes, colLogicalIndexes);
            CollectionAssert.AreEqual(expectedLogicalIndexes, propLogicalIndexes);
        }

        [TestMethod]
        public void ShouldPassCorrectIndexesWhenWriting()
        {
            var data = new[] { new Person { Id = 1, Name = "Bob", CreatedOn = new DateTime(2018, 06, 30), IsActive = true } };
            var mapper = SeparatedValueTypeMapper.Define(() => new Person());
            var colPhysicalIndexes = new List<int>();
            var colLogicalIndexes = new List<int>();
            var propPhysicalIndexes = new List<int>();
            var propLogicalIndexes = new List<int>();
            mapper.CustomMapping(new IndexTrackingColumn(new Int32Column("Id"), colPhysicalIndexes, colLogicalIndexes))
                .WithWriter((ctx, x) =>
                {
                    propPhysicalIndexes.Add(ctx.PhysicalIndex);
                    propLogicalIndexes.Add(ctx.LogicalIndex);
                    return x.Id;
                });
            mapper.Ignored();
            mapper.CustomMapping(new IndexTrackingColumn(new StringColumn("Name"), colPhysicalIndexes, colLogicalIndexes))
                .WithWriter((ctx, x) =>
                {
                    propPhysicalIndexes.Add(ctx.PhysicalIndex);
                    propLogicalIndexes.Add(ctx.LogicalIndex);
                    return x.Name;
                });
            mapper.Ignored();
            mapper.CustomMapping(new IndexTrackingColumn(new DateTimeColumn("CreatedOn") { InputFormat = "yyyy-MM-dd" }, colPhysicalIndexes, colLogicalIndexes))
                .WithWriter((ctx, x) =>
                {
                    propPhysicalIndexes.Add(ctx.PhysicalIndex);
                    propLogicalIndexes.Add(ctx.LogicalIndex);
                    return x.CreatedOn;
                });
            mapper.Ignored();
            mapper.CustomMapping(new IndexTrackingColumn(new BooleanColumn("IsActive"), colPhysicalIndexes, colLogicalIndexes))
                .WithWriter((ctx, x) =>
                {
                    propPhysicalIndexes.Add(ctx.PhysicalIndex);
                    propLogicalIndexes.Add(ctx.LogicalIndex);
                    return x.IsActive;
                });
            var writer = new StringWriter();
            var typedWriter = mapper.GetWriter(writer);
            typedWriter.WriteAll(data);

            var expectedPhysicalIndexes = new int[] { 0, 2, 4, 6 };
            CollectionAssert.AreEqual(expectedPhysicalIndexes, colPhysicalIndexes);
            CollectionAssert.AreEqual(expectedPhysicalIndexes, propPhysicalIndexes);

            var expectedLogicalIndexes = new int[] { 0, 1, 2, 3 };
            CollectionAssert.AreEqual(expectedLogicalIndexes, colLogicalIndexes);
            CollectionAssert.AreEqual(expectedLogicalIndexes, propLogicalIndexes);
        }

        internal class Person
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public DateTime CreatedOn { get; set; }

            public bool IsActive { get; set; }
        }

        internal class IndexTrackingColumn : IColumnDefinition
        {
            private readonly IColumnDefinition column;
            private readonly List<int> physicalIndexes;
            private readonly List<int> logicalIndexes;

            public IndexTrackingColumn(
                IColumnDefinition columnDefinition,
                List<int> physicalIndexes,
                List<int> logicalIndexes)
            {
                this.column = columnDefinition;
                this.physicalIndexes = physicalIndexes;
                this.logicalIndexes = logicalIndexes;
            }

            public string ColumnName => column.ColumnName;

            public bool IsIgnored => column.IsIgnored;

            public bool IsNullable => column.IsNullable;

            public IDefaultValue DefaultValue
            {
                get => column.DefaultValue;
                set => column.DefaultValue = value;
            }

            public INullFormatter NullFormatter
            {
                get => column.NullFormatter;
                set => column.NullFormatter = value;
            }

            [Obsolete]
            public Func<string, string> Preprocessor
            {
                get => column.Preprocessor;
                set => column.Preprocessor = value;
            }

            public Func<IColumnContext, string, string> OnParsing
            {
                get => column.OnParsing;
                set => column.OnParsing = value;
            }

            public Func<IColumnContext, object, object> OnParsed
            {
                get => column.OnParsed;
                set => column.OnParsed = value;
            }

            public Func<IColumnContext, object, object> OnFormatting
            {
                get => column.OnFormatting;
                set => column.OnFormatting = value;
            }

            public Func<IColumnContext, string, string> OnFormatted
            {
                get => column.OnFormatted;
                set => column.OnFormatted = value;
            }

            public Type ColumnType => column.ColumnType;

            public string Format(IColumnContext context, object value)
            {
                physicalIndexes.Add(context.PhysicalIndex);
                logicalIndexes.Add(context.LogicalIndex);
                return column.Format(context, value);
            }

            public object Parse(IColumnContext context, string value)
            {
                physicalIndexes.Add(context.PhysicalIndex);
                logicalIndexes.Add(context.LogicalIndex);
                return column.Parse(context, value);
            }
        }
    }
}
