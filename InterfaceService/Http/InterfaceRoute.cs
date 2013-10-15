using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace InterfaceService.Http
{
    public class InterfaceRoute<T> : Route
    {
        public InterfaceRoute(IStringSerializer serializer, string url, Func<T> instanceProvider)
            : base(url, new RouteValueDictionary(new { func = "" }), new InterfaceRouteHandler<T>(serializer, instanceProvider))
        {
        }

        public InterfaceRoute(string url, Func<T> instanceProvider)
            : base(url, new RouteValueDictionary(new { func = "" }), new InterfaceRouteHandler<T>(new JsonSerializer(), instanceProvider))
        {
        }
    }

    public class InterfaceHttpHandler<T> : IHttpHandler
    {
        private readonly Func<T> _instanceProvider;
        private readonly string _function;
        private readonly IStringSerializer _serializer;
        public InterfaceHttpHandler(IStringSerializer serializer, string function, Func<T> instanceProvider)
        {
            _instanceProvider = instanceProvider;
            _function = function;
            _serializer = serializer;
        }
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var qs = context.Request.QueryString;
            var qsParameters = context.Request.QueryString.AllKeys.Select(x => new KeyValuePair<string, object>(x, context.Request.QueryString[x]));

            //TODO: Handle Get/QueryString

            string content = "";
            using (var reader = new StreamReader(context.Request.InputStream)) {
                content = reader.ReadToEnd();
            }

            var parameters = _serializer.DeserializeParameters(content);

            var method = MethodFinder.FindMethod<T>(_function, parameters);
            var result = method.CallOn(_instanceProvider());

            context.Response.Write(_serializer.SerializeResponse(result));
        }
    }

    public class InterfaceRouteHandler<T> : IRouteHandler
    {
        private readonly Func<T> _instanceProvider;
        private readonly IStringSerializer _serializer;
        public InterfaceRouteHandler(IStringSerializer serializer, Func<T> instanceProvider)
        {
            _instanceProvider = instanceProvider;
            _serializer = serializer;
        }
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var q = requestContext.RouteData.Values["func"] as string;
            if (String.IsNullOrWhiteSpace(q)) { throw new ArgumentException("Display usage or contract or something"); }

            return new InterfaceHttpHandler<T>(_serializer, q, _instanceProvider);
        }
    }


}
