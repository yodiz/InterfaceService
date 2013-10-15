using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceService
{
    public interface IStringSerializer
    {
        string SerializeParameter(string[] parameterNames, object[] parameterValues);
        IEnumerable<IParameter> DeserializeParameters(string parameterResponse);

        string SerializeResponse(object value);
        object DeserializeResponse(string response, Type expectedType);
    }    
}
