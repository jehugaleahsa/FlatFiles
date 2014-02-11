using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FlatFiles.TypeMapping;

namespace FlatFiles.Mvc
{
    public class SeparatedValueResult<T> : ActionResult
    {
        private readonly ISeparatedValueTypeWriter<T> writer;
        private readonly SeparatedValueOptions options;
        private readonly string fileName;
        private readonly string contentType;

        public SeparatedValueResult(ISeparatedValueTypeMapper<T> mapper, IEnumerable<T> data, SeparatedValueOptions options = null, string fileName = null, string contentType = null)
        {
            if (mapper == null)
            {
                throw new ArgumentNullException("mapper");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
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

        public SeparatedValueResult(ISeparatedValueTypeWriter<T> writer, SeparatedValueOptions options = null, string fileName = null, string contentType = null)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (options == null)
            {
                options = new SeparatedValueOptions();
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
            context.HttpContext.Response.AddHeader("content-disposition", "attachment;  filename=" + fileName);
            writer.Write(context.HttpContext.Response.OutputStream, options);
        }
    }

    public static class SeparatedValueResult
    {
        public static SeparatedValueResult<T> Create<T>(ISeparatedValueTypeMapper<T> mapper, IEnumerable<T> data, SeparatedValueOptions options = null, string fileName = null, string contentType = null)
        {
            return new SeparatedValueResult<T>(mapper, data, options, fileName);
        }

        public static SeparatedValueResult<T> Create<T>(ISeparatedValueTypeWriter<T> writer, SeparatedValueOptions options = null, string fileName = null, string contentType = null)
        {
            return new SeparatedValueResult<T>(writer, options, fileName);
        }
    }
}
