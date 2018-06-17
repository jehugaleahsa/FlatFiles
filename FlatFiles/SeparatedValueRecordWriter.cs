using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlatFiles.Properties;

namespace FlatFiles
{
    internal sealed class SeparatedValueRecordWriter
    {
        private readonly TextWriter _writer;
        private readonly SeparatedValueSchemaInjector _injector;
        private readonly string _quoteString;
        private readonly string _doubleQuoteString;

        public SeparatedValueRecordWriter(TextWriter writer, SeparatedValueSchema schema, SeparatedValueOptions options)
        {
            _writer = writer;
            Metadata = new SeparatedValueMetadata
            {
                Schema = schema,
                Options = options.Clone()
            };
            _quoteString = string.Empty + options.Quote;
            _doubleQuoteString = string.Empty + options.Quote + options.Quote;
        }

        public SeparatedValueRecordWriter(TextWriter writer, SeparatedValueSchemaInjector injector, SeparatedValueOptions options)
            : this(writer, (SeparatedValueSchema)null, options)
        {
            _injector = injector;
        }

        public SeparatedValueMetadata Metadata { get; }

        public void WriteRecord(object[] values)
        {
            var schema = GetSchema(values);
            if (schema != null && values.Length != schema.ColumnDefinitions.PhysicalCount)
            {
                throw new ArgumentException(Resources.WrongNumberOfValues, nameof(values));
            }
            var formattedValues = FormatValues(schema, values);
            EscapeValues(formattedValues);
            string joined = string.Join(Metadata.Options.Separator, formattedValues);
            _writer.Write(joined);
        }

        public async Task WriteRecordAsync(object[] values)
        {
            var schema = GetSchema(values);
            if (schema != null && values.Length != schema.ColumnDefinitions.PhysicalCount)
            {
                throw new ArgumentException(Resources.WrongNumberOfValues, nameof(values));
            }
            var formattedValues = FormatValues(schema, values);
            EscapeValues(formattedValues);
            string joined = string.Join(Metadata.Options.Separator, formattedValues);
            await _writer.WriteAsync(joined).ConfigureAwait(false);
        }

        private SeparatedValueSchema GetSchema(object[] values)
        {
            return _injector == null ? Metadata.Schema : _injector.GetSchema(values);
        }

        private string[] FormatValues(SeparatedValueSchema schema, object[] values)
        {
            if (schema == null)
            {
                string[] results = new string[values.Length];
                for (int index = 0; index != results.Length; ++index)
                {
                    results[index] = ToString(values[index]);
                }
                return results;
            }

            var metadata = _injector == null ? Metadata : new SeparatedValueMetadata
            {
                Schema = schema,
                Options = Metadata.Options,
                RecordCount = Metadata.RecordCount,
                LogicalRecordCount = Metadata.LogicalRecordCount
            };
            return schema.FormatValues(Metadata, values);
        }

        private static string ToString(object value)
        {
            return value == null ? string.Empty : value.ToString();
        }

        private void EscapeValues(string[] values)
        {
            for (int index = 0; index != values.Length; ++index)
            {
                values[index] = Escape(values[index]);
            }
        }

        private string Escape(string value)
        {
            if (NeedsEscaped(value))
            {
                return _quoteString + value.Replace(_quoteString, _doubleQuoteString) + _quoteString;
            }

            return value;
        }

        private bool NeedsEscaped(string value)
        {
            // Never escape null.
            if (value == null)
            {
                return false;
            }
            if (Metadata.Options.QuoteBehavior == QuoteBehavior.AlwaysQuote)
            {
                return true;
            }
            // Don't escape empty strings.
            if (value == string.Empty)
            {
                return false;
            }
            // Escape strings beginning or ending in whitespace.
            if (char.IsWhiteSpace(value[0]) || char.IsWhiteSpace(value[value.Length - 1]))
            {
                return true;
            }
            // Escape strings containing the separator.
            if (value.Contains(Metadata.Options.Separator))
            {
                return true;
            }
            // Escape strings containing the record separator.
            if (Metadata.Options.RecordSeparator != null && value.Contains(Metadata.Options.RecordSeparator))
            {
                return true;
            }
            // Escape strings containing quotes.
            if (value.Contains(_quoteString))
            {
                return true;
            }
            return false;
        }

        public void WriteSchema()
        {
            if (_injector != null)
            {
                return;
            }
            if (Metadata.Schema == null)
            {
                return;
            }
            var names = Metadata.Schema.ColumnDefinitions.Select(d => Escape(d.ColumnName));
            string joined = string.Join(Metadata.Options.Separator, names);
            _writer.Write(joined);
        }

        public async Task WriteSchemaAsync()
        {
            if (_injector != null)
            {
                return;
            }
            if (Metadata.Schema == null)
            {
                return;
            }
            var names = Metadata.Schema.ColumnDefinitions.Select(d => Escape(d.ColumnName));
            string joined = string.Join(Metadata.Options.Separator, names);
            await _writer.WriteAsync(joined).ConfigureAwait(false);
        }

        public void WriteRecordSeparator()
        {
            _writer.Write(Metadata.Options.RecordSeparator ?? Environment.NewLine);
        }

        public async Task WriteRecordSeparatorAsync()
        {
            await _writer.WriteAsync(Metadata.Options.RecordSeparator ?? Environment.NewLine).ConfigureAwait(false);
        }

        internal class SeparatedValueMetadata : IProcessMetadata
        {
            public SeparatedValueSchema Schema { get; internal set; }

            ISchema IProcessMetadata.Schema => Schema;

            public SeparatedValueOptions Options { get; internal set; }

            IOptions IProcessMetadata.Options => Options;

            public int RecordCount { get; internal set; }

            public int LogicalRecordCount { get; internal set; }
        }
    }
}
