using FlatFiles;
using FlatFiles.TypeMapping;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Drg.M3.Client
{
    public class FixedLengthResult<T> : ActionResult
    {
        IFixedLengthTypeMapper<T> _mapper;
        IEnumerable<T> _data;
        string _name;
        FixedLengthOptions _options;

        public FixedLengthResult(IFixedLengthTypeMapper<T> mapper, IEnumerable<T> data, string name, FixedLengthOptions options)
        {
            _mapper = mapper;
            _data = data;
            _name = name;
            _options = options;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "text/plain";
            context.HttpContext.Response.AddHeader("content-disposition", "attachment;  filename=" + _name + ".txt");

            _mapper.Write(context.HttpContext.Response.OutputStream, _options, _data);
        }
    }

    public static class FixedLengthResult
    {
        public static FixedLengthResult<T> Create<T>(IFixedLengthTypeMapper<T> mapper, IEnumerable<T> data, string name, FixedLengthOptions options)
        {
            return new FixedLengthResult<T>(mapper, data, name, options);
        }
    }
}
