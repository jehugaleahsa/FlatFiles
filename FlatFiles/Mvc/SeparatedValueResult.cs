using FlatFiles;
using FlatFiles.TypeMapping;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Drg.M3.Client
{
    public class SeparatedValueResult<T> : ActionResult
    {
        ISeparatedValueTypeMapper<T> _mapper;
        IEnumerable<T> _data;
        string _name;
        SeparatedValueOptions _options;

        public SeparatedValueResult(ISeparatedValueTypeMapper<T> mapper, IEnumerable<T> data, string name, SeparatedValueOptions options)
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

    public static class SeparatedValueResult
    {
        public static SeparatedValueResult<T> Create<T>(ISeparatedValueTypeMapper<T> mapper, IEnumerable<T> data, string name, SeparatedValueOptions options)
        {
            return new SeparatedValueResult<T>(mapper, data, name, options);
        }
    }
}
