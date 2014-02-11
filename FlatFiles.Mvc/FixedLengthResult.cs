using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FlatFiles.TypeMapping;

namespace FlatFiles.Mvc
{
    public class FixedLengthResult<T> : ActionResult
    {
        private readonly IFixedLengthTypeWriter<T> writer;
        private readonly FixedLengthOptions options;
        private readonly string fileName;
        private readonly string contentType;

        public FixedLengthResult(IFixedLengthTypeMapper<T> mapper, IEnumerable<T> data, FixedLengthOptions options = null, string fileName = null, string contentType = null)
        {
            if (mapper == null)
            {
                throw new ArgumentNullException("mapper");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (options == null)
            {
                options = new FixedLengthOptions();
            }
            if (String.IsNullOrWhiteSpace(fileName))
            {
                fileName = "file.txt";
            }
            if (contentType == null)
            {
                contentType = "text/plain";
            }
            this.writer = mapper.ToWriter(data);
            this.options = options;
            this.fileName = fileName;
            this.contentType = contentType;
        }

        public FixedLengthResult(IFixedLengthTypeWriter<T> writer, FixedLengthOptions options = null, string fileName = null, string contentType = null)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (options == null)
            {
                options = new FixedLengthOptions();
            }
            if (String.IsNullOrWhiteSpace(fileName))
            {
                fileName = "file.txt";
            }
            if (contentType == null)
            {
                contentType = "text/plain";
            }
            this.writer = writer;
            this.options = options;
            this.fileName = fileName;
            this.contentType = contentType;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = contentType;
            context.HttpContext.Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            writer.Write(context.HttpContext.Response.OutputStream, options);
        }
    }

    public static class FixedLengthResult
    {
        public static FixedLengthResult<T> Create<T>(IFixedLengthTypeMapper<T> mapper, IEnumerable<T> data, FixedLengthOptions options = null, string fileName = null, string mimeType = null)
        {
            return new FixedLengthResult<T>(mapper, data, options, fileName, mimeType);
        }

        public static FixedLengthResult<T> Create<T>(IFixedLengthTypeWriter<T> writer, FixedLengthOptions options = null, string fileName = null, string mimeType = null)
        {
            return new FixedLengthResult<T>(writer, options, fileName, mimeType);
        }
    }
}
