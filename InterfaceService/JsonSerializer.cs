using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InterfaceService
{
    public class JsonSerializer : IStringSerializer
    {
        private class JsonParameter : IParameter
        {
            private string p;
            private JToken jToken;

            public JsonParameter(string p, JToken jToken) { this.p = p; this.jToken = jToken; }
            public string Name { get { return p; } }
            public object GetValue(Type type) { return jToken.ToObject(type); }
        }

        public string SerializeParameter(string[] parameterNames, object[] parameterValues)
        {
            if (parameterNames.Length != parameterValues.Length) throw new ApplicationException("Waaaaat?");
            return SerializeResponse(new { Values = parameterValues, Names = parameterNames });
        }

        public IEnumerable<IParameter> DeserializeParameters(string parameterResponse)
        {
            var test = Newtonsoft.Json.Linq.JObject.Parse(parameterResponse);

            var names = test.Property("Names").Values();
            var values = test.Property("Values").Values();

            return names.Zip(values, (a, b) => new JsonParameter(a.ToObject<string>(), b)).ToArray();
        }

        public string SerializeResponse(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public object DeserializeResponse(string response, Type expectedType)
        {
            return JsonConvert.DeserializeObject(response, expectedType);
        }
    }
}
