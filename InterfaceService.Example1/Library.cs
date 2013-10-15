using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceService.Example1
{
    public class ComplexInput
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public interface IInterface
    {
        string Hello(string name);
        string Test(int num);
        ComplexInput Comples(string a, int num);
        ComplexInput Comples2(ComplexInput b);
    }
}
