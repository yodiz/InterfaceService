using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceService
{
    public class MethodMap<T>
    {
        private readonly IEnumerable<ParameterMap> ParameterMap;
        private readonly MethodInfo _methodInfo;
        public MethodMap(MethodInfo methodInfo, IEnumerable<ParameterMap> paramMap)
        {
            ParameterMap = paramMap;
            _methodInfo = methodInfo;
        }

        public object CallOn(T instance)
        {
            var parameters = ParameterMap.Select(x => x.Value).ToArray();
            return _methodInfo.Invoke(instance, parameters);
        }
    }

    public class ParameterMap
    {
        public ParameterMap(string parameterName, object value)
        {
            Value = value;
        }

        public readonly object Value;
    }

    public interface IParameter
    {
        string Name { get; }
        object GetValue(Type type);
    }

    public class MethodFinder
    {
        public static MethodMap<T> FindMethod<T>(string function, IEnumerable<IParameter> parameters)
        {
            //Find function with name and matching parameters
            var methods = typeof(T).GetMethods();
            var q = methods.Select(x =>
            {
                var methodParams = x.GetParameters();
                var parameterMap =
                    methodParams.Select(p =>
                    {
                        var param = parameters.Where(i => i.Name == p.Name);
                        if (param.Any())
                        {
                            var _p = param.First();
                            return new ParameterMap(_p.Name, _p.GetValue(p.ParameterType));
                        }
                        return null;
                    });

                if (x.Name != function || methodParams.Length != parameters.Count())
                {
                    return null;
                }

                if (parameterMap.Contains(null))
                {
                    return null;
                }

                return new MethodMap<T>(x, parameterMap);
            }).Where(x => x != null);

            if (q.Count() > 1) throw new ArgumentException("Multiple signatues found");
            if (q.Count() == 0) throw new ArgumentException("No signatue found");

            return q.First();
        }
    }

}
