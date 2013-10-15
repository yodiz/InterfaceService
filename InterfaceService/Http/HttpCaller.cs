
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace InterfaceService.Http
{
    public enum HttpSerialization { Json = 1 }
    public class HttpInterface<T>
    {
        public T Client { private set; get; }

        public HttpInterface(HttpSerialization serialization, string url)
        {
            var test = new ProxyInterface<T>(new Http.HttpCaller(new JsonSerializer(), url));
            Client = test.Client;
        }

        public HttpInterface(string url) : this(HttpSerialization.Json, url) { }
    }

    public class HttpCaller : IMethodCaller
    {
        public string Url { get; private set; }
        public IStringSerializer IHttpSerializer { get; private set; }
        public HttpCaller(IStringSerializer serializer, string url)
        {
            this.Url = url;
            this.IHttpSerializer = serializer;
        }

        public object Call(string methodName, string[] parameterNames, object[] parameterValues, Type[] parameterTypes, Type expectedReturn)
        {
            //var formater = new JsonHttpSerizlizer();
            //TODO: handle Get/post

            string response = null;
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                var requestBody = IHttpSerializer.SerializeParameter(parameterNames, parameterValues);
                response = client.UploadString(Url + methodName, "post", requestBody);
            }

            if (typeof(void) == expectedReturn) { return null; }
            var value = IHttpSerializer.DeserializeResponse(response, expectedReturn);
            return value;
        }
    }



}
